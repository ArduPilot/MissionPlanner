namespace Xamarin.Controls
{
    public class Cursors
    {
        public static Cursor Hand { get; set; } = new Cursor();
        public Cursor Current { get; set; } = new Cursor();
        public static Cursor Default { get; set; } = new Cursor();
        public static Cursor SizeAll { get; set; } = new Cursor();
    }

    public class Cursor
    {
        public Cursor Current { get; set; }
    }
}