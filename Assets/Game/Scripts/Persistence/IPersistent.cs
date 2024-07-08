// Interface class for persistent objects
public interface IPersistent
{
    // Serialize the relevant data and return it
    public byte[] GetSaveData();

    // Parse the serialized data and load it into object properties
    public void LoadSaveData(byte[] data);
}
