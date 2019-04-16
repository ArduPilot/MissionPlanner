namespace MissionPlanner.Utilities.Drawing
{
    public enum CombineMode
    {
        /// <summary>One clipping region is replaced by another.</summary>
        Replace,
        /// <summary>Two clipping regions are combined by taking their intersection.</summary>
        Intersect,
        /// <summary>Two clipping regions are combined by taking the union of both.</summary>
        Union,
        /// <summary>Two clipping regions are combined by taking only the areas enclosed by one or the other region, but not both.</summary>
        Xor,
        /// <summary>Specifies that the existing region is replaced by the result of the new region being removed from the existing region. Said differently, the new region is excluded from the existing region.</summary>
        Exclude,
        /// <summary>Specifies that the existing region is replaced by the result of the existing region being removed from the new region. Said differently, the existing region is excluded from the new region.</summary>
        Complement
    }
}