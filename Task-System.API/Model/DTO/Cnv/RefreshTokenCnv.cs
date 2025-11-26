using Task_System.Model.Entity;

namespace Task_System.Model.DTO.Cnv;

public class RefreshTokenCnv
{
    public RefreshTokenDto EntityToDto( RefreshToken refreshToken)
    {
        return new RefreshTokenDto(
            Token: refreshToken.Token,
            Expires: refreshToken.Expires
        );
    }
}
