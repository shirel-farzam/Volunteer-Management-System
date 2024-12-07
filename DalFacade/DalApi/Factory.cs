namespace DalApi;
public static class Factory
{
	public static IDal Get
	{
    	get
    	{
        	string dalType = DalApi.DalConfig.s_dalName ?? throw new DalConfigException($"DAL name is not extracted from the configuration");
        	DalApi.DalConfig.DalImplementation dal = DalApi.DalConfig.s_dalPackages[dalType] ?? throw new DalConfigException($"Package for {dalType} is not found in packages list in dal-config.xml");
 
        	try { System.Reflection.Assembly.Load(dal.Package ?? throw new DalConfigException($"Package {dal.Package} is null")); }
        	catch (Exception ex) { throw new DalConfigException($"Failed to load {dal.Package}.dll package", ex); }
 
        	Type type = Type.GetType($"{dal.Namespace}.{dal.Class}, {dal.Package}") ??
            	throw new DalConfigException($"Class {dal.Namespace}.{dal.Class} was not found in {dal.Package}.dll");
 
        	return type.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null) as IDal ??
            	throw new DalConfigException($"Class {dal.Class} is not a singleton or wrong property name for Instance");
    	}
	}
}
