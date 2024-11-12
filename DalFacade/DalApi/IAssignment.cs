namespace DalApi;
using DO;
public interface IAssignment
{
    void Create(IAssignment item); //Creates new entity object in DAL
    IAssignment? Read(int id); //Reads entity object by its ID 
    List<IAssignment> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(IAssignment item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects

}
