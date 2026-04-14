namespace ZooBuilder.ui.settings;

public interface ISetting
{
    string Key { get; }
    object GetValue();
    void SetValue(object value);
}