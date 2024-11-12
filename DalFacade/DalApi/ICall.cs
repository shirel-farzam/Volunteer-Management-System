

namespace DalApi;
using DO;


public interface ICall
{
    void Create(ICall item); //Creates new entity object in DAL
    ICall? Read(int id); //Reads entity object by its ID 
    List<ICall> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(ICall item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects

}
