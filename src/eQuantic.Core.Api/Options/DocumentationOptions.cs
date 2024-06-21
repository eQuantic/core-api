using System.Reflection;

namespace eQuantic.Core.Api.Options;

public class DocumentationOptions
{
    public DocumentationOptions WithTitle(string title)
    {
        Title = title;
        return this;
    }

    public DocumentationOptions WithXmlCommentsFile(string fileName)
    {
        XmlCommentsFile = fileName;
        return this;
    }
    
    public DocumentationOptions FromAssembly(Assembly assembly)
    {
        Assembly = assembly;
        if (string.IsNullOrEmpty(XmlCommentsFile))
            XmlCommentsFile = $"{Assembly.GetName().Name}.xml";
        
        return this;
    }

    public DocumentationOptions WithAuthorization(Action<DocumentationAuthorizationOptions> options)
    {
        var docAuthOptions = new DocumentationAuthorizationOptions();
        options.Invoke(docAuthOptions);

        AuthorizationOptions = docAuthOptions;
        
        return this;
    }

    public DocumentationOptions WithSignIn(string signInUrl)
    {
        SignInUrl = signInUrl;
        return this;
    }
    
    internal string? Title { get; private set; }
    internal string? XmlCommentsFile { get; private set; }
    internal Assembly? Assembly { get; private set; }
    internal DocumentationAuthorizationOptions? AuthorizationOptions { get; private set; }
    internal string? SignInUrl { get; private set; }
}

public class DocumentationAuthorizationOptions
{
    public string? Description { get; set; }
}