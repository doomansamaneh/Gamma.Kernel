namespace Gamma.Kernel.Dapper;

public static class SQL
{
    public static SqlBuilder WITH(string format, params object[] args)
    {
        //todo: 
        //return new SqlBuilder().WITH(format, args);
        return new();
    }

    public static SqlBuilder Select(string format)
    {
        return new SqlBuilder().Select(format);
    }

    public static SqlBuilder Update(string format)
    {
        return new SqlBuilder().Update(format);
    }

    public static SqlBuilder DeleteFrom(string format)
    {
        return new SqlBuilder().DeleteFrom(format);
    }

    #region Object Members

    /// <exclude/>

    public static new bool Equals(object objectA, object objectB)
    {
        return Object.Equals(objectA, objectB);
    }

    /// <exclude/>
    public static new bool ReferenceEquals(object objectA, object objectB)
    {
        return Object.ReferenceEquals(objectA, objectB);
    }

    #endregion
}

