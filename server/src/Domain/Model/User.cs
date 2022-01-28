#pragma warning disable 8618
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using Domain.Exceptions;

namespace Domain.Model
{
    public class User : Entity, IAggregateRoot
    {
        private User(string username, string email, string passwordHash, string encryptionKeyHash,
            string emailVerificationTokenHash, DateTime emailVerificationTokenValidTo, string universalToken)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            EncryptionKeyHash = encryptionKeyHash;
            IdEmailConfirmed = false;
            EmailVerificationTokenHash = emailVerificationTokenHash;
            EmailVerificationTokenValidTo = emailVerificationTokenValidTo;
            UniversalToken = universalToken;
        }

        private User(string username, string email, string passwordHash, string encryptionKeyHash,
            string universalToken)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            EncryptionKeyHash = encryptionKeyHash;
            UniversalToken = universalToken;
            IdEmailConfirmed = false;
        }

        public long Id { get; private init; }
        public string Username { get; private init; }
        public string Email { get; private init; }
        public string UniversalToken { get; init; }
        public string PasswordHash { get; private set; }
        public string EncryptionKeyHash { get; private set; }
        public string? PasswordResetTokenHash { get; private set; }
        public DateTime? PasswordResetTokenValidTo { get; private set; }
        public bool IdEmailConfirmed { get; private set; }
        public string? EmailVerificationTokenHash { get; private set; }
        public DateTime? EmailVerificationTokenValidTo { get; private set; }

        public static User Register(string username, string email, string passwordHash, string encryptionKeyHash,
            string emailVerificationTokenHash, DateTime emailVerificationTokenValidTo, string universalToken)
        {
            return new User(username, email, passwordHash, encryptionKeyHash, emailVerificationTokenHash,
                emailVerificationTokenValidTo, universalToken);
        }

        public void ResetPassword(string passwordHash)
        {
            PasswordHash = passwordHash;
            PasswordResetTokenHash = null;
            PasswordResetTokenValidTo = null;
        }

        public void VerifyEmail()
        {
            IdEmailConfirmed = true;
            EmailVerificationTokenHash = null;
            EmailVerificationTokenValidTo = null;
        }

        public void AddPasswordResetToken(string passwordResetTokenHash, DateTime passwordResetTokenValidTo)
        {
            PasswordResetTokenHash = passwordResetTokenHash;
            PasswordResetTokenValidTo = passwordResetTokenValidTo;
        }

        public void ChangePassword(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public void UpdateEmailVerificationToken(string verificationTokenHash, DateTime verificationTokenValidTo)
        {
            EmailVerificationTokenHash = verificationTokenHash;
            EmailVerificationTokenValidTo = verificationTokenValidTo;
        }
    }
}