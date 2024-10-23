using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Features.User.Entities;
using Features.User.DTOs;
public class UserList : List<User>, IEnumerable<User>
{
    public UserList(List<User> users) : base(users) {}

    public IEnumerable<UserDTO> GetUserListDTO()
    {
        return this.Select(user => new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            PublicRank = Enum.TryParse(user.Rank, out Rank parsedRank) ? parsedRank : Rank.None
        });
    }




    public IEnumerable<User> GetUserListByRank(Rank rank)
    {
        foreach(var user in this) {
            if (Enum.TryParse(user.Rank, out Rank userRank) && userRank == rank)
            {
                yield return user;
            }
        }
    }

    public bool CheckUsernameAvailability(string username)
    {
        foreach(var user in this)
        {
            if(user.DisplayName == username){
                return false;
            }
        }
        return true;
    }

    IEnumerator<User> IEnumerable<User>.GetEnumerator()
    {
        return base.GetEnumerator();
    }

    //Overide
    IEnumerator IEnumerable.GetEnumerator()
    {
        return base.GetEnumerator();
    }
}
