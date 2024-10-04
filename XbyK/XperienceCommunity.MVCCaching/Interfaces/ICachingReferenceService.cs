namespace MVCCaching
{
    public interface ICachingReferenceService
    {
        /// <summary>
        /// Gets the Language Name give the Content Language ID, useful since most Dependency Keys for language are by name but the ID is returned in the System Fields
        /// </summary>
        /// <param name="websiteChannelId"></param>
        /// <returns>The Language Name</returns>
        string GetLanguageNameById(int languageId);

        /// <summary>
        /// Given the website channel ID, returns the default language name
        /// </summary>
        /// <param name="websiteChannelId"></param>
        /// <returns>The Default Language Name</returns>
        string GetDefaultLanguageName(int websiteChannelId);

        /// <summary>
        /// Gets the Channel Name by the ChannelID
        /// </summary>
        /// <param name="channelId">The Channel ID</param>
        /// <returns>The Channel Name</returns>
        string GetChannelNameByChannelId(int channelId);

        /// <summary>
        /// Gets the Channel Name by the given Website Channel ID
        /// </summary>
        /// <param name="websiteChannelId">Website Channel ID</param>
        /// <returns>The Channel Name</returns>
        string GetChannelNameByWebsiteChannelId(int websiteChannelId);

        /// <summary>
        /// Gets the current Web Channel's Name
        /// </summary>
        /// <returns>The Current Web Channel's Name</returns>
        string GetCurrentWebChannelName();
    }
}
