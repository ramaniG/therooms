﻿using Fmarketer.Base;
using Fmarketer.Base.Enums;
using Fmarketer.DataAccess.Repository;
using Fmarketer.Models;
using Fmarketer.Models.Dto;
using Fmarketer.Models.Model;
using System;
using System.Threading.Tasks;

namespace Fmarketer.Business
{
    public class AuthenticationBU
    {
        CredentialRepository credentialRepository;
        SecurityTokenRepository securityTokenRepository;

        UnitOfWork unitOfWork;

        public AuthenticationBU(UnitOfWork unit, CredentialRepository credentialRepository, SecurityTokenRepository securityTokenRepository)
        {
            this.credentialRepository = credentialRepository;
            this.securityTokenRepository = securityTokenRepository;

            unitOfWork = unit;
        }

        public async Task<LoginOutDto> LoginByEmailAsync(LoginDto dto)
        {
            var credential = await credentialRepository.FindByEmailAsync(dto.Email);

            if (credential != null && credential.AuthType == AUTHTYPES.Email && credential.CredentialState == CREDENTIALSTATUS.Active && credential.Verified) {
                if (BCrypt.BCryptHelper.CheckPassword(dto.Password, credential.Password)) {
                    credential.LastLogin = DateTime.Now;
                    credentialRepository.Update(credential);

                    var token = await CreateSecurityTokenAsync(credential);

                    await unitOfWork.Complete();

                    return new LoginOutDto(credential, token);
                }
            }

            throw new InvalidOperationException(ErrorMessage.USERMGMT_OPERATION_FAILED);
        }

        public async Task LogoutByEmailAsync(LogoutDto dto)
        {
            var token = await securityTokenRepository.Get(new Guid(dto.Token));

            if (token != null) {
                token.ExpiryTime = DateTime.Now;

                securityTokenRepository.Update(token);

                await unitOfWork.Complete();
                return;
            }

            throw new InvalidOperationException(ErrorMessage.USERMGMT_OPERATION_FAILED);
        }

        private async Task<SecurityToken> CreateSecurityTokenAsync(Credential credential)
        {
            var token = new SecurityToken() {
                Id = Guid.NewGuid(),
                AuthenticatedTime = credential.LastLogin,
                ExpiryTime = credential.LastLogin.AddMinutes(15), // TODO: Change Minutes to setting file
                _Credential = credential
            };

            token = await securityTokenRepository.AddAsync(token);
            await unitOfWork.Complete();

            return token;
        }
    }
}
