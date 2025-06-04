using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Dapper;

using MRRS.Shared.Entities;

namespace MRRS.Model
{
    /// <summary>
    /// 場所管理画面のModelクラスです。
    /// </summary>
    public class LocationListModel
    {
        /// <summary>
        /// 場所一覧。
        /// </summary>
        public BindingList<Location> Locations { get; private set; }

        /// <summary>
        /// 初期処理を行います。
        /// </summary>
        public void Initialize()
        {
            using (var conn = DbProvider.CreateConnection())
            {
                var locations = conn.SelectLocations();
                Locations = new BindingList<Location>(locations.ToList());
            }
        }

        /// <summary>
        /// 場所を更新します。
        /// </summary>
        public void Save()
        {
            using (var conn = DbProvider.CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                conn.Execute("delete from LOCATION", transaction: tran);
                foreach (var loc in Locations)
                {
                    conn.Execute(@"insert into LOCATION(ID, NAME) values(SEQ_LOCATION_ID.NEXTVAL, :Name)", loc, transaction: tran);
                }
                tran.Commit();
            }
        }

        /// <summary>
        /// 指定場所を削除してよいか判定します。
        /// </summary>
        /// <param name="locationId">場所ID。</param>
        /// <returns>削除可能な場合true。</returns>
        public bool CanDelete(int? locationId)
        {
            if (!locationId.HasValue) return true;
            using (var conn = DbProvider.CreateConnection())
            {
                var count = conn.ExecuteScalar<int>(
                    "select count(*) from MEETING_ROOM where LOCATION_ID = :Id",
                    new { Id = locationId });
                return count == 0;
            }
        }
    }
}
