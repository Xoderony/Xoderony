namespace GuestUnion.Int32Extensions {

    public static class Int32Extensions {

        public static int Clamp(this int value, int min, int max) =>
            value < min
            ? min
            : value > max ? max : value;
    }
}