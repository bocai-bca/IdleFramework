## 放置引擎-空间
## 每个在游戏资源中被提交注册的空间都会在游戏启动时被创建一个实例，可通过核心搭配ID来获取空间实例
extends Resource
class_name IdleFramework_Space

## ID，该空间的唯一标识ID。
## 此项留空将使该物品在注册时失效
@export var id: StringName

func make_instance() -> IdleFramework_SpaceInstance:
	var result: IdleFramework_SpaceInstance = IdleFramework_SpaceInstance.new()
	result.id =
	return result
