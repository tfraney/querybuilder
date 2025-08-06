namespace SqlKata
{
    public static class VERB
    {
        public const string PushField = "*";
        public const string StartParenth = "(";
        public const string EndParenth = ")";
        public const string And = "AND";
        public const string And2 = "&&";
        public const string And3 = "&";
        public const string Or = "OR";
        public const string Or2 = "||";
        public const string Or3 = "|";
        public const string Or4 = "<>";
        public const string CaseWhen = "CASE WHEN";
        public const string When = "WHEN";
        public const string Else = "ELSE";
        public const string Then = "THEN";
        public const string End = "END";
        public const string As = "AS";
        public const string Asc = "ASC";
        public const string Desc = "DESC";

        public const string RowNumberPartitioned = "ROW_NUMBER() OVER (PARTITION BY";
        public const string RowNumberOrderBy = "ROW_NUMBER() OVER (ORDER BY";
        public const string RowNumber = "ROW_NUMBER()";
        public const string OrderBy = "ORDER BY";


        public static string[] SpecialChar = [StartParenth, EndParenth, PushField];
        public static string[] AndOrOpertors = [And, And2, And3, Or, Or2, Or3, Or4];
        public static string[] AndOpertors = [And, And2, And3];
        public static string[] OrOpertors = [Or, Or2, Or3, Or4];
    }
}
