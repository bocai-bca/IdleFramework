## 放置引擎
extends IdleFramework_NumberProvider
class_name IdleFramework_NumberProvier_Direct

## 数字，将固定提供该数字
@export var number: int

func _get_number() -> int:
	return number
