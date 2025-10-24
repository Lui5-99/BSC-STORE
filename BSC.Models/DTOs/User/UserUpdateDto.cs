using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.DTOs.User
{
    public class UserUpdateDto
    {
        public string Username { get; set; } = string.Empty;

		[MinLength(8, ErrorMessage = "La contrase�a debe tener al menos 8 caracteres.")]
		[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$", ErrorMessage = "Debe contener letras, n�meros y al menos un car�cter especial.")]
		public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}
