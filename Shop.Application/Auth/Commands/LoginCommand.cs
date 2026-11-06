using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Auth.Commands;

public record LoginCommand(string Username, string Password);