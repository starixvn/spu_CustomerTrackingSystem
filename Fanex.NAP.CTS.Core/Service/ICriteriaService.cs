namespace Fanex.NAP.CTS.Core.Service
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using System.Collections.Generic;

    public interface ICriteriaService
    {
        T Get<T>(ICriteria criteria);

        int Execute(INonQueryCommand command);

        IEnumerable<T> Fetch<T>(ICriteria criteria);

        (IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet) Fetch<TFirst, TSecond>(ICriteria criteria);

        (IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet, IEnumerable<TThird> thirdResultSet) Fetch<TFirst, TSecond, TThird>(ICriteria criteria);

        DBResult UpdateData(CommandBase criteria, string prefix);
    }
}
