namespace EmployeeAuditTest
{
    public static class HelperMethods
    {
        public static bool AreEqual<T>(T obj1, T obj2)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var value1 = property.GetValue(obj1);
                var value2 = property.GetValue(obj2);

                if (!Equals(value1, value2))
                {
                    return false;
                }
            }

            return true;
        }
    }
}