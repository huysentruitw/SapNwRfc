using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SapNwRfc
{
    /// <summary>
    /// Represents the SAP connection parameters for passing settings to the <see cref="SapConnection"/> class.
    /// </summary>
    public class SapConnectionParameters
    {
        /// <summary>
        /// Gets or sets the Application Server Host parameter.
        /// </summary>
        [SapName("ASHOST")]
        public string AppServerHost { get; set; }

        /// <summary>
        /// Gets or sets the SNC Library Path parameter.
        /// </summary>
        [SapName("SNC_LIB")]
        public string SncLibraryPath { get; set; }

        /// <summary>
        /// Gets or sets the SNC SSO parameter.
        /// </summary>
        [SapName("SNC_SSO")]
        public string SncSso { get; set; }

        /// <summary>
        /// Gets or sets the SNC QOP parameter.
        /// </summary>
        [SapName("SNC_QOP")]
        public string SncQop { get; set; }

        /// <summary>
        /// Gets or sets the Trace parameter.
        /// </summary>
        [SapName("TRACE")]
        public string Trace { get; set; }

        /// <summary>
        /// Gets or sets the SAP Router parameter.
        /// </summary>
        [SapName("SAPROUTER")]
        public string SapRouter { get; set; }

        /// <summary>
        /// Gets or sets the No Compression parameter.
        /// </summary>
        [SapName("NO_COMPRESSION")]
        public string NoCompression { get; set; }

        /// <summary>
        /// Gets or sets the On Character Conversion Error parameter.
        /// </summary>
        [SapName("ON_CCE")]
        public string OnCharacterConversionError { get; set; }

        /// <summary>
        /// Gets or sets the Character Fault Indicator Token parameter.
        /// </summary>
        [SapName("CFIT")]
        public string CharacterFaultIndicatorToken { get; set; }

        /// <summary>
        /// Gets or sets the Maximum Pool Size parameter.
        /// </summary>
        [SapName("MAX_POOL_SIZE")]
        public string MaxPoolSize { get; set; }

        /// <summary>
        /// Gets or sets the Pool Size parameter.
        /// </summary>
        [SapName("POOL_SIZE")]
        public string PoolSize { get; set; }

        /// <summary>
        /// Gets or sets the SNC Partner Names parameter.
        /// </summary>
        [SapName("SNC_PARTNER_NAMES")]
        public string SncPartnerNames { get; set; }

        /// <summary>
        /// Gets or sets the Idle Timeout parameter.
        /// </summary>
        [SapName("IDLE_TIMEOUT")]
        public string IdleTimeout { get; set; }

        /// <summary>
        /// Gets or sets the Maximum Pool Wait Time parameter.
        /// </summary>
        [SapName("MAX_POOL_WAIT_TIME")]
        public string MaxPoolWaitTime { get; set; }

        /// <summary>
        /// Gets or sets the Registration Count parameter.
        /// </summary>
        [SapName("REG_COUNT")]
        public string RegistrationCount { get; set; }

        /// <summary>
        /// Gets or sets the Password Change Enforced parameter.
        /// </summary>
        [SapName("PASSWORD_CHANGE_ENFORCED")]
        public string PasswordChangeEnforced { get; set; }

        /// <summary>
        /// Gets or sets the Name parameter.
        /// </summary>
        [SapName("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Repository Destination parameter.
        /// </summary>
        [SapName("REPOSITORY_DESTINATION")]
        public string RepositoryDestination { get; set; }

        /// <summary>
        /// Gets or sets the Repository User parameter.
        /// </summary>
        [SapName("REPOSITORY_USER")]
        public string RepositoryUser { get; set; }

        /// <summary>
        /// Gets or sets the Repository Password parameter.
        /// </summary>
        [SapName("REPOSITORY_PASSWD")]
        public string RepositoryPassword { get; set; }

        /// <summary>
        /// Gets or sets the Repository SNC My Name parameter.
        /// </summary>
        [SapName("REPOSITORY_SNC_MYNAME")]
        public string RepositorySncMyName { get; set; }

        /// <summary>
        /// Gets or sets the Reporitory X509 Certificate parameter.
        /// </summary>
        [SapName("REPOSITORY_X509CERT")]
        public string RepositoryX509Certificate { get; set; }

        /// <summary>
        /// Gets or sets the Idle Check Time parameter.
        /// </summary>
        [SapName("IDLE_CHECK_TIME")]
        public string IdleCheckTime { get; set; }

        /// <summary>
        /// Gets or sets the SNC My Name parameter.
        /// </summary>
        [SapName("SNC_MYNAME")]
        public string SncMyName { get; set; }

        /// <summary>
        /// Gets or sets the SNC Partner Name parameter.
        /// </summary>
        [SapName("SNC_PARTNERNAME")]
        public string SncPartnerName { get; set; }

        /// <summary>
        /// Gets or sets the Program Id parameter.
        /// </summary>
        [SapName("PROGRAM_ID")]
        public string ProgramId { get; set; }

        /// <summary>
        /// Gets or sets the App Server Service parameter.
        /// </summary>
        [SapName("ASSERV")]
        public string AppServerService { get; set; }

        /// <summary>
        /// Gets or sets the Message Server Host parameter.
        /// </summary>
        [SapName("MSHOST")]
        public string MessageServerHost { get; set; }

        /// <summary>
        /// Gets or sets the Message Server Service parameter.
        /// </summary>
        [SapName("MSSERV")]
        public string MessageServerService { get; set; }

        /// <summary>
        /// Gets or sets the R/3 Name parameter.
        /// </summary>
        [SapName("R3NAME")]
        public string R3Name { get; set; }

        /// <summary>
        /// Gets or sets the Logon Group parameter.
        /// </summary>
        [SapName("GROUP")]
        public string LogonGroup { get; set; }

        /// <summary>
        /// Gets or sets the Gateway Host parameter.
        /// </summary>
        [SapName("GWHOST")]
        public string GatewayHost { get; set; }

        /// <summary>
        /// Gets or sets the Gateway Service parameter.
        /// </summary>
        [SapName("GWSERV")]
        public string GatewayService { get; set; }

        /// <summary>
        /// Gets or sets the System Number parameter.
        /// </summary>
        [SapName("SYSNR")]
        public string SystemNumber { get; set; }

        /// <summary>
        /// Gets or sets the User parameter.
        /// </summary>
        [SapName("USER")]
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the Alias User parameter.
        /// </summary>
        [SapName("ALIAS_USER")]
        public string AliasUser { get; set; }

        /// <summary>
        /// Gets or sets the SNC Mode parameter.
        /// </summary>
        [SapName("SNC_MODE")]
        public string SncMode { get; set; }

        /// <summary>
        /// Gets or sets the Client parameter.
        /// </summary>
        [SapName("CLIENT")]
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets the Password parameter.
        /// </summary>
        [SapName("PASSWD")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the Codepage parameter.
        /// </summary>
        [SapName("CODEPAGE")]
        public string Codepage { get; set; }

        /// <summary>
        /// Gets or sets the Partner Char Size parameter.
        /// </summary>
        [SapName("PCS")]
        public string PartnerCharSize { get; set; }

        /// <summary>
        /// Gets or sets the System ID parameter.
        /// </summary>
        [SapName("SYSID")]
        public string SystemId { get; set; }

        /// <summary>
        /// Gets or sets the Systems ID parameter.
        /// </summary>
        [SapName("SYS_IDS")]
        public string SystemIds { get; set; }

        /// <summary>
        /// Gets or sets the X509 Certificate parameter.
        /// </summary>
        [SapName("X509CERT")]
        public string X509Certificate { get; set; }

        /// <summary>
        /// Gets or sets the SAP SSO2 Ticket parameter.
        /// </summary>
        [SapName("MYSAPSSO2")]
        public string SapSso2Ticket { get; set; }

        /// <summary>
        /// Gets or sets the Use SAP GUI parameter.
        /// </summary>
        [SapName("USE_SAPGUI")]
        public string UseSapGui { get; set; }

        /// <summary>
        /// Gets or sets the ABAP Debug parameter.
        /// </summary>
        [SapName("ABAP_DEBUG")]
        public string AbapDebug { get; set; }

        /// <summary>
        /// Gets or sets the Logon Check parameter.
        /// </summary>
        [SapName("LCHECK")]
        public string LogonCheck { get; set; }

        /// <summary>
        /// Gets or sets the Language parameter.
        /// </summary>
        [SapName("LANG")]
        public string Language { get; set; }

        /// <summary>
        /// Parses a connection string into a <see cref="SapConnectionParameters"/> object.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The <see cref="SapConnectionParameters"/> instance.</returns>
        public static SapConnectionParameters Parse(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Value cannot be null or empty", nameof(connectionString));

            IReadOnlyDictionary<string, string> parts = connectionString
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(entry => Regex.Match(entry, @"^\s*(?<key>[^\s=]+)\s*=\s*(?<value>\S+)\s*$"))
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
