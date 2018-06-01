namespace SharpKml.Dom
{
    /// <summary>
    /// Used to signify an element contains HTML content and its children/inner
    /// text should not be parsed.
    /// </summary>
    internal interface IHtmlContent
    {
        /// <summary>Gets or sets the content of this instance.</summary>
        /// <remarks>The value may contain well formed HTML.</remarks>
        string Text { get; set; }
    }
}
