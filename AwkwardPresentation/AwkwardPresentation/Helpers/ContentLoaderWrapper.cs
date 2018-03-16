using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Logging;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.PageExtensions;

namespace VisitOslo.Infrastructure.Helpers
{
    /// <summary>
    /// Helper class for locating child/descendant pages.
    /// </summary>
    public class ContentLoaderWrapper
    {
        #region Static fields
        // Used for all logging. Uses the EPiServer Logging API, not any logging plugins API directly.
        private static readonly ILogger _logger = LogManager.GetLogger();
        #endregion

        #region Fields
        private readonly IContentLoader _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
        #endregion

        #region Constructor
        public ContentLoaderWrapper()
        {
        }
        #endregion

        // Primary difference between page and block methods are that block methods use BlockData instead of IContent, and have overloads with CultureInfo
        #region Page methods

        /// <summary>
        /// Convenience method to get a page from the <paramref name="reference"/> parameter.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="culture"></param>
        /// <returns>Object of type <typeparam name="T">T</typeparam>, or null</returns>
        public T GetPageFromReference<T>(ContentReference reference, CultureInfo culture = null) where T : IContent
        {
            if (reference == null || reference.ID == 0)
            {
                return default(T);
            }   

            try
            {
                return _contentLoader.Get<T>(reference, culture);
            }
            catch (TypeMismatchException e)
            {
                _logger.Log(Level.Error, $"Type mismatch when retrieving a page of type {typeof(T).FullName}.", e);
            }
            catch (ContentNotFoundException e)
            {
                _logger.Log(Level.Debug, $"Could not find the content we're looking for. The reference parameter is {reference}", e);
            }

            // Returns null if we get here, because T has to be of type IContent, i.e. reference to an object, and thus nullable.
            return default(T);
        }

        /// <summary>
        /// Convenience method to make a list of ContentReference into a list of the pagetype sent in.
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="culture"></param>
        /// <returns>An empty list of type <typeparam name="T">T</typeparam></returns>
        public IEnumerable<T> GetPagesFromReferenceList<T>(IEnumerable<ContentReference> pages, CultureInfo culture = null) where T : IContent
        {
            if (pages.IsNullOrEmpty())
            {
                return new List<T>();
            }

            try
            {
                return pages?.Select(p => _contentLoader.Get<T>(p, culture));
            }
            catch (TypeMismatchException e)
            {
                _logger.Log(Level.Error, $"Type mismatch when retrieving a page of type {typeof(T).FullName}.", e);
                return new List<T>();
            }
            catch (Exception e) when (e is ArgumentNullException || e is ContentNotFoundException)
            {
                _logger.Log(Level.Debug, $"Argument not found, or could not find the content we're looking for. Input list of pages is {pages.Count()} long.", e);
                return new List<T>();
            }
        }



        /// <summary>
        /// Finds all pages of a page type below <paramref name="pageLink"/>, and runs it through FilterForVisitor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageLink">PageReference to search below</param>
        /// <param name="recursive">Whether to search recursively</param>
        /// <returns>A list containing items of type <typeparam name="T">T</typeparam> or an empty list</returns>
        public IEnumerable<T> FindFilteredPagesOfType<T>(ContentReference pageLink, bool recursive) where T : IContent
        {
            return FindFilteredPagesOfType<T>(pageLink, recursive, null);
        }

        /// <summary>
        /// Finds all pages of a page type below <paramref name="pageLink"/>, and runs it through FilterForVisitor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageLink">PageReference to search below</param>
        /// <param name="recursive">Whether to search recursively</param>
        /// <param name="culture"></param>
        /// <returns>A list containing items of type <typeparam name="T">T</typeparam> or an empty list</returns>
        public IEnumerable<T> FindFilteredPagesOfType<T>(ContentReference pageLink, bool recursive, CultureInfo culture) where T : IContent
        {
            if (pageLink == null || pageLink.ID == 0)
            {
                return new List<T>();
            }

            var subPages = FindPagesOfType<T>(pageLink, recursive, culture).Cast<IContent>();

            var filterAcces = new FilterAccess(AccessLevel.Read);
            var publishedFilter = new FilterPublished();

            var filteredSubPages = subPages
                .Where(p => !filterAcces.ShouldFilter(p))
                .Where(p => !publishedFilter.ShouldFilter(p));

            // FilterForVisitor.Filter doesn't work for some reason. 
            //It returns null, even if the exact code it uses (tested through reflection) does return the correct list of items.
            //var filteredSubPages = FilterForVisitor.Filter(subPages);

            return filteredSubPages.Cast<T>();
        }


        /// <summary>
        /// Finds all pages of a page type below a PageReference. Does not filter on anything.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageLink">PageReference to search below</param>
        /// <param name="recursive">Whether to search recursively</param>
        /// <returns></returns>
        public IEnumerable<T> FindPagesOfType<T>(ContentReference pageLink, bool recursive) where T : IContent
        {
            return FindPagesOfType<T>(pageLink, recursive, null);
        }

        /// <summary>
        /// Finds all pages of a page type below a PageReference. Does not filter on anything.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageLink">PageReference to search below</param>
        /// <param name="recursive">Whether to search recursively</param>
        /// <param name="culture">The culture of the page to search.</param>
        /// <returns></returns>
        public IEnumerable<T> FindPagesOfType<T>(ContentReference pageLink, bool recursive, CultureInfo culture) where T : IContent
        {
            if (pageLink == null || pageLink.ID == 0)
            {
                return new List<T>();
            }

            // If culture isn't set, just retrieve pages the normal way. 
            if (culture == null)
            {
                return (recursive)
                    ? _contentLoader.GetDescendents(pageLink)
                        .Where(p => _contentLoader.Get<IContent>(p) is T)
                        .Select(_contentLoader.Get<T>)
                    : _contentLoader.GetChildren<T>(pageLink);
            }

            // If culture is set, find the pages filtering on culture.
            // Could probably combine this with the above culture-ignorant calls, but don't have time to test these calls properly for pages not using separate languages.
            if (recursive)
            {
                var pages = GetPagesFromReferenceList<T>(_contentLoader.GetDescendents(pageLink));
                return pages.Where(p => p.LanguageBranch().Equals(culture.TwoLetterISOLanguageName)).ToList();
            }
            else
            {
                return _contentLoader.GetChildren<T>(pageLink, culture);
            }
        }

        #endregion


        #region Block methods
        /// <summary>
        /// Convenience method to get a block from the <paramref name="reference"/> parameter.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns>Object of type <typeparam name="T">T</typeparam>, or null</returns>
        public T GetBlockFromReference<T>(ContentReference reference) where T : BlockData
        {
            if (reference == null || reference.ID == 0)
            {
                return null;
            }

            try
            {
                return _contentLoader.Get<T>(reference);
            }
            catch (TypeMismatchException e)
            {
                _logger.Log(Level.Error, $"Type mismatch when retrieving a block of type {typeof(T).FullName}.", e);
            }
            catch (ContentNotFoundException e)
            {
                _logger.Log(Level.Debug, $"Could not find the content we're looking for. The reference parameter is {reference}", e);
            }

            // Returns null if we get here, because T has to be of type BlockData, i.e. reference to an object, and thus nullable.
            return null;
        }

        /// <summary>
        /// Convenience method to make a list of ContentReference into a list of the blocktype sent in.
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns>An empty list of type <typeparam name="T">T</typeparam></returns>
        public IEnumerable<T> GetBlocksFromReferenceList<T>(IEnumerable<ContentReference> blocks) where T : BlockData
        {
            if (blocks.IsNullOrEmpty())
            {
                return new List<T>();
            }

            try
            {
                return blocks?.Select(p => _contentLoader.Get<T>(p));
            }
            catch (TypeMismatchException e)
            {
                _logger.Log(Level.Error, $"Type mismatch when retrieving a block of type {typeof(T).FullName}.", e);
                return new List<T>();
            }
            catch (Exception e) when (e is ArgumentNullException || e is ContentNotFoundException)
            {
                _logger.Log(Level.Debug, $"Argument not found, or could not find the content we're looking for. Input list of blocks is {blocks.Count()} long.", e);
                return new List<T>();
            }
        }

        /// <summary>
        /// Finds all blocks of a block type below <paramref name="reference"/>, and runs it through FilterForVisitor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference">ContentReference to search below</param>
        /// <param name="recursive">Whether to search recursively</param>
        /// <returns>A list containing items of type <typeparam name="T">T</typeparam>, or null</returns>
        public IEnumerable<T> FindFilteredBlocksOfType<T>(ContentReference reference, bool recursive) where T : BlockData
        {
            if (reference == null || reference.ID == 0)
            {
                return new List<T>();
            }

            var subPages = FindBlocksOfType<T>(reference, recursive).Cast<BlockData>();

            var filterAcces = new FilterAccess(AccessLevel.Read);
            var publishedFilter = new FilterPublished();

            var filteredBlocks = subPages
                .Where(b => !filterAcces.ShouldFilter((IContent)b))
                .Where(b => !publishedFilter.ShouldFilter((IContent)b));

            // FilterForVisitor.Filter doesn't work for some reason. 
            //It returns null, even if the exact code it uses (tested through reflection) does return the correct list of items.
            //var filteredSubPages = FilterForVisitor.Filter(subPages);

            return filteredBlocks.Cast<T>();
        }

        /// <summary>
        /// Finds all blocks of a block type below <paramref name="reference"/>. Does not filter on anything.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference">PageReference to search below</param>
        /// <param name="recursive">Whether to search recursively</param>
        /// <returns>A list containing items of type <typeparam name="T">T</typeparam>, or null</returns>
        public IEnumerable<T> FindBlocksOfType<T>(ContentReference reference, bool recursive) where T : BlockData
        {
            if (reference == null || reference.ID == 0)
            {
                return new List<T>();
            }

            return (recursive)
                ? _contentLoader.GetDescendents(reference)
                    .Where(p => _contentLoader.Get<BlockData>(p) is T)
                    .Select(_contentLoader.Get<T>)
                : _contentLoader.GetChildren<T>(reference);
        }
        #endregion


        #region Project-specific methods
      
        #endregion



    }
}