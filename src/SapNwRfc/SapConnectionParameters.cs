using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SapNwRfc
{
    public class SapConnectionParameters
    {
        [SapName("ASHOST")]
        public string AppServerHost { get; set; }

        [SapName("SNC_LIB")]
        public string SncLibraryPath { get; set; }

        [SapName("SNC_QOP")]
        public string SncQop { get; set; }

        [SapName("TRACE")]
        public string Trace { get; set; }

        [SapName("SAPROUTER")]
        public string SapRouter { get; set; }

        [SapName("NO_COMPRESSION")]
        public string NoCompression { get; set; }

        [SapName("ON_CCE")]
        public string OnCharacterConversionError { get; set; }

        [SapName("CFIT")]
        public string CharacterFaultIndicatorToken { get; set; }

        [SapName("MAX_POOL_SIZE")]
        public string MaxPoolSize { get; set; }

        [SapName("POOL_SIZE")]
        public string PoolSize { get; set; }

        [SapName("SNC_PARTNER_NAMES")]
        public string SncPartnerNames { get; set; }

        [SapName("IDLE_TIMEOUT")]
        public string IdleTimeout { get; set; }

        [SapName("MAX_POOL_WAIT_TIME")]
        public string MaxPoolWaitTime { get; set; }

        [SapName("REG_COUNT")]
        public string RegistrationCount { get; set; }

        [SapName("PASSWORD_CHANGE_ENFORCED")]
        public string PasswordChangeEnforced { get; set; }

        [SapName("NAME")]
        public string Name { get; set; }

        [SapName("REPOSITORY_DESTINATION")]
        public string RepositoryDestination { get; set; }

        [SapName("REPOSITORY_USER")]
        public string RepositoryUser { get; set; }

        [SapName("REPOSITORY_PASSWD")]
        public string RepositoryPassword { get; set; }

        [SapName("REPOSITORY_SNC_MYNAME")]
        public string RepositorySncMyName { get; set; }

        [SapName("REPOSITORY_X509CERT")]
        public string RepositoryX509Certificate { get; set; }

        [SapName("IDLE_CHECK_TIME")]
        public string IdleCheckTime { get; set; }

        [SapName("SNC_MYNAME")]
        public string SncMyName { get; set; }

        [SapName("SNC_PARTNERNAME")]
        public string SncPartnerName { get; set; }

        [SapName("PROGRAM_ID")]
        public string ProgramId { get; set; }

        [SapName("ASSERV")]
        public string AppServerService { get; set; }

        [SapName("MSHOST")]
        public string MessageServerHost { get; set; }

        [SapName("MSSERV")]
        public string MessageServerService { get; set; }

        [SapName("GROUP")]
        public string LogonGroup { get; set; }

        [SapName("GWHOST")]
        public string GatewayHost { get; set; }

        [SapName("GWSERV")]
        public string GatewayService { get; set; }

        [SapName("SYSNR")]
        public string SystemNumber { get; set; }

        [SapName("USER")]
        public string User { get; set; }

        [SapName("ALIAS_USER")]
        public string AliasUser { get; set; }

        [SapName("SNC_MODE")]
        public string SncMode { get; set; }

        [SapName("CLIENT")]
        public string Client { get; set; }

        [SapName("PASSWD")]
        public string Password { get; set; }

        [SapName("CODEPAGE")]
        public string Codepage { get; set; }

        [SapName("PCS")]
        public string PartnerCharSize { get; set; }

        [SapName("SYSID")]
        public string SystemId { get; set; }

        [SapName("SYS_IDS")]
        public string SystemIds { get; set; }

        [SapName("X509CERT")]
        public string X509Certificate { get; set; }

        [SapName("MYSAPSSO2")]
        public string SapSso2Ticket { get; set; }

        [SapName("USE_SAPGUI")]
        public string UseSapGui { get; set; }

        [SapName("ABAP_DEBUG")]
        public string AbapDebug { get; set; }

        [SapName("LCHECK")]
        public string LogonCheck { get; set; }

        [SapName("LANG")]
        public string Language { get; set; }

        public static SapConnectionParameters Parse(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Value cannot be null or empty", nameof(connectionString));

            IReadOnlyDictionary<string, string> parts = connectionString
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(entry => Regex.Match(entry, @"^\s*(?<key>\S+)\s*=\s*(?<value>\S+)\s*$"))
                .Where(match => match.Success)
                .ToDictionary(match => match.Groups["key"].Value, match => match.Groups["value"].Value);

            return typeof(SapConnectionParameters)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Aggregate(new SapConnectionParameters(), (parameters, propertyInfo) =>
                {
                    if (parts.ContainsKey(propertyInfo.Name) && propertyInfo.CanWrite)
                        propertyInfo.SetValue(parameters, parts[propertyInfo.Name]);
                    return parameters;
                });
        }
    }
}
