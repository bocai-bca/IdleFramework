## 放置引擎-空间
## 每个在游戏资源中被提交注册的空间都会在游戏启动时被创建一个实例，可通过核心搭配ID来获取空间实例
extends Resource
class_name IdleFramework_Space

## 空间用途，主要用于告诉官方UI场景如何使用此空间
enum UsedFor{
	TITLE_BAR, ## 顶部栏物品显示
	SIDE_BAR, ## 呈现为侧边栏的一个项目，用于定义一个页面
	BACKGROUND, ## 存在于后台而不会显示，仅用于存储物品
}

## 预装容器，如果不在此指定要装载的容器，则后续必须通过代码向空间实例中添加容器实例
## 键为容器ID，值为容器
@export var preset_containers: Dictionary[StringName, IdleFramework_Container]

## 预装工厂，如果不在此指定要装载的工厂，则后续必须通过代码向空间实例中添加工厂实例
## 键为工厂ID，值为工厂
@export var preset_factories: Dictionary[StringName, IdleFramework_Factory]

## 名称翻译标识符，即翻译表中该空间名称的键。
@export var name_key: StringName

## 图标，该空间显示在GUI上的图标，将基于引擎设置自动生成Mipmap
@export var icon: Texture2D

## 描述翻译标识符，即翻译表中该空间描述的键
@export var lore_key: StringName

## 空间用途
@export var used_for: UsedFor = UsedFor.BACKGROUND

func make_instance() -> IdleFramework_SpaceInstance:
	var new_instance: IdleFramework_SpaceInstance = IdleFramework_SpaceInstance.new()
	for id in preset_containers.keys():
		new_instance.containers[id] = preset_containers[id].make_instance()
	for id in preset_factories.keys():
		new_instance.factories[id] = preset_factories[id].make_instance()
	new_instance.icon = icon
	new_instance.name_key = name_key
	new_instance.lore_key = lore_key
	new_instance.used_for = used_for
	return new_instance

func preset_containers_(new_preset_containers: Dictionary[StringName, IdleFramework_Container]) -> IdleFramework_Space:
	preset_containers = new_preset_containers
	return self

func preset_factories_(new_preset_factories: Dictionary[StringName, IdleFramework_Factory]) -> IdleFramework_Space:
	preset_factories = new_preset_factories
	return self

func name_key_(new_key: StringName) -> IdleFramework_Space:
	name_key = new_key
	return self

func icon_(new_icon: Texture2D) -> IdleFramework_Space:
	icon = new_icon
	return self

func lore_key_(new_key: StringName) -> IdleFramework_Space:
	lore_key = new_key
	return self
