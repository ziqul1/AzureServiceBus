using FluentValidation;
using QueueSenderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueReceiver
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Name).NotNull();
            RuleFor(user => user.Surname).NotNull();
            RuleFor(user => user.Email).NotNull().EmailAddress();
            RuleFor(user => user.Age).NotNull();
        }
    }
}
