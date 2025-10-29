using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public record ConfirmEmailChangeDTO(string userId, string email, string token) { }
}
