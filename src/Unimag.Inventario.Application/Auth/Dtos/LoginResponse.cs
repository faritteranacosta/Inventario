namespace Unimag.Inventario.Application.Auth.Dtos;
public sealed record LoginResponse(string Token, Unimag.Inventario.Application.Users.Dtos.UserResponse User);