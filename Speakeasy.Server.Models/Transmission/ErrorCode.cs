namespace Speakeasy.Server.Models.Transmission;

public enum ErrorCode
{
    EntityNotFound,
    MissingParameterId,
    EmptyParameterExpectedId,
    UploadedFileLengthNotValid,
    UploadedImageNotValid,
    UploadedImageTooBig,
    RoleAlreadyExists,
    NotEnoughRolesLeft,
    NoDefaultRolesLeft,
    NameAlreadyTaken,
    TooManyMessageReactions
}