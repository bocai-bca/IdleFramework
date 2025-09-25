## 放置引擎-物品注册表对象，用于在游戏资源中注册物品
extends Resource
class_name IdleFramework_ItemRegistryObject

## ID，该物品的唯一标识ID\n
## 此项留空将使该物品在注册时失效
@export var id: StringName

## 名称翻译标识符，即翻译表中该物品名称的键\n
## 此项留空将使该物品没有名称
@export var name_key: StringName

## 图标，该物品显示在GUI上的图标，将基于引擎设置自动生成Mipmap
@export var icon: Texture2D

## 描述翻译标识符，即翻译表中该物品描述的键
@export var lore_key: StringName

## 默认最大数量，该数值作用于容器中的物品，同ID的物品在容器中最多只能存放此数量个，如果某一容器有配置物品的最大数量，那么在该容器中容器配置的数值将覆盖此处设置的数值进行生效
@export var default_max_count: int = 9999

## 设置ID，用于通过代码创建实例时链式调用
func id_(new_id: StringName) -> IdleFramework_ItemRegistryObject:
	id = new_id
	return self

## 设置名称翻译标识符，用于通过代码创建实例时链式调用
func name_key_(new_name_key: StringName) -> IdleFramework_ItemRegistryObject:
	name_key = new_name_key
	return self

## 设置图标，用于通过代码创建实例时链式调用
func icon_(new_icon: Texture2D) -> IdleFramework_ItemRegistryObject:
	icon = new_icon
	return self

## 设置描述翻译标识符，用于通过代码创建实例时链式调用
func lore_key_(new_lore_key: StringName) -> IdleFramework_ItemRegistryObject:
	lore_key = new_lore_key
	return self

## 设置默认最大数量
func default_max_count_(new_count: int) -> IdleFramework_ItemRegistryObject:
	default_max_count = new_count
	return self
