namespace libs.Utils
{
    public static class ObjectUtils
    {
        /// <summary>
        /// Returns an alternative if the current object is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thing"></param>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public static T Or<T>(this T thing, T alternate)
        {
            if (thing == null)
            {
                return alternate;
            }

            return thing;
        }
    }
}
