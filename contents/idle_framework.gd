extends Object
class_name IdleFramework
## 放置引擎声明类，本类中定义其他部分可以用到的类型和方法，本身不允许实例化

func _init() -> void:
	call_deferred(&"free")
	push_error("IdleFramework: Instantiating this class is not be allowed.")

## 类-游戏资源
class GameResource extends RefCounted:
	var item_registry: Array[IdleFramework.Item]

## 类-物品，声明继承自该类的子类并覆写相关虚函数，使其返回新指定的值，以定义一个物品
class Item extends Object:
	static func _get_id() -> String:
		return ""
	static func _get_translate_key() -> String:
		return ""
