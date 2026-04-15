using System;

namespace ZooBuilder.ui.settings;

public interface ISetting
{
    string Key { get; }
    object GetValue();
    void SetValue(object newValue);
    void executeOnSaveCallback();
}