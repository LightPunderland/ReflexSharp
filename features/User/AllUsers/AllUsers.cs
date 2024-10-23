using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Features.User.Entities;
using Features.User.DTOs;

// AHSAHSAHSAHSHAHSAHSH XDDDDDD
public class UserList : List<User>, IEnumerable<User>, IComparable<UserList>
{
    public UserList(List<User> users) : base(users) {}

    // IEnumerable implement cause we have to
    public new IEnumerator<User> GetEnumerator()
    {
        foreach(var user in this)
        {
            yield return user;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Icomparable...
    // 1 - otherList is greater
    // -1 - otherList is lesser
    // 0 - they are equal
    public int CompareTo(UserList otherList)
    {
        if (otherList == null)
        {
            return 1;
        }

        return this.Count.CompareTo(otherList.Count);
    }

    // We compare users based off ranks (can be changed to any other criteria)
    public class UserComparer : IComparer<User>
    {
        public int Compare(User a, User b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            Rank rankOfA = Enum.TryParse(a.Rank, out Rank parsedRankA) ? parsedRankA : Rank.None;
            Rank rankOfB = Enum.TryParse(b.Rank, out Rank parsedRankB) ? parsedRankB : Rank.None;

            return rankOfA.CompareTo(rankOfB);
        }
    }

}
