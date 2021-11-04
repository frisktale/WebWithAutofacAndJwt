using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace WebWithAutofacAndJwt.Entity
{
    public class User : IdentityUser<long>
    {
        
    }
}