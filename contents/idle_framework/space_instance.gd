## 放置引擎-空间实例
extends RefCounted
class_name IdleFramework_SpaceInstance

## 容器实例
## 键为容器ID，值为容器实例
var containers: Dictionary[StringName, IdleFramework_ContainerInstance]

## 工厂实例
## 键为工厂ID，值为工厂实例
var factories: Dictionary[StringName, IdleFramework_FactoryInstance]

## 名称翻译标识符，即翻译表中该空间名称的键。
var name_key: StringName

## 图标，该空间显示在GUI上的图标，将基于引擎设置自动生成Mipmap
var icon: Texture2D

## 描述翻译标识符，即翻译表中该空间描述的键
var lore_key: StringName

## 空间用途
var used_for: IdleFramework_Space.UsedFor
