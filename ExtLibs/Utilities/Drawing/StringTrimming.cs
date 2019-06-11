namespace MissionPlanner.Utilities.Drawing
{
    public enum StringTrimming
    {
        /// <summary>Specifies no trimming.</summary>
        None,
        /// <summary>Specifies that the text is trimmed to the nearest character.</summary>
        Character,
        /// <summary>Specifies that text is trimmed to the nearest word.</summary>
        Word,
        /// <summary>Specifies that the text is trimmed to the nearest character, and an ellipsis is inserted at the end of a trimmed line.</summary>
        EllipsisCharacter,
        /// <summary>Specifies that text is trimmed to the nearest word, and an ellipsis is inserted at the end of a trimmed line.</summary>
        EllipsisWord,
        /// <summary>The center is removed from trimmed lines and replaced by an ellipsis. The algorithm keeps as much of the last slash-delimited segment of the line as possible.</summary>
        EllipsisPath
    }
}