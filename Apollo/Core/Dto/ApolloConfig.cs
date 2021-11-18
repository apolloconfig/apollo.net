namespace Com.Ctrip.Framework.Apollo.Core.Dto;

public class ApolloConfig
{
    public string AppId { get; set; } = default!;

    public string Cluster { get; set; } = default!;

    public string NamespaceName { get; set; } = default!;

    public string ReleaseKey { get; set; } = default!;

    public IDictionary<string, string>? Configurations { get; set; }

    public override string ToString() => $"ApolloConfig{{appId='{AppId}{'\''}, cluster='{Cluster}{'\''}, namespaceName='{NamespaceName}{'\''}, configurations={Configurations}, releaseKey='{ReleaseKey}{'\''}{'}'}";
}