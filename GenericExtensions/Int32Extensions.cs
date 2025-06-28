namespace GuestUnion {

    public static class Int32Extensions {

        public static int Abs(this int value) => value < 0 ? -value : value;

        public static int Clamp(this int value, int min, int max) =>
            value < min
            ? min
            : value > max ? max : value;

        public static bool HasFlag(this int value, int flag) => (value & flag) == flag;
    }
}