using System.ComponentModel;
using System.Text;

namespace AnnaFest
{
    public interface IUser
    {
        public string Name { get; }

        public bool IsAdmin { get; }

        public bool Exists { get; }
    }

    public class User : IUser
    {
        public static IUser Current(ISession session)
        {
            session.TryGetValue("Name", out var name);
            if (name == null)
            {
                return new NullUser();
            } 
            else
            {
                session.TryGetValue("IsAdmin", out var isAdmin);
                return new User
                {
                    Name = Encoding.ASCII.GetString(name),
                    IsAdmin = isAdmin[0] == 1
                };
            }
        }

        public static void SetUser(ISession session, string name, bool isAdmin)
        {
            session.Set("Name", Encoding.ASCII.GetBytes(name));
            session.Set("IsAdmin", isAdmin ? new byte[] { 1 } : new byte[] { 0 });
        }

        public string Name { get; set; }

        public bool IsAdmin { get; set; }

        public bool Exists => true;
    }

    public class NullUser : IUser
    {
        public string Name => null;

        public bool IsAdmin => false;

        public bool Exists => false;
    }
}
