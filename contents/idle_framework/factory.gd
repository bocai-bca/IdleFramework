## 放置引擎-工厂，一个工厂的实例可以执行一个配方
extends Resource
class_name IdleFramework_Factory

## 生产类型
enum ProductionType{
	MANUALLY_START, ## 手动开始，需要点击一次工厂来启动生产，随时间增加进度
	MANUALLY_PROCESS, ## 手动生产，需要通过点击工厂来增加进度，每点击一次增加一秒的进度，每次数据更新只能应用一次点击
	AUTO, ## 自动，将自动开始生产并随时间增加进度
}

## 需求类型
enum RequireType{
	START_NEED_INGREDIENTS, ## 开始生产需要满足原材料，如果中途不满足将导致进度清空
	PROCESS_NEED_INGREDIENTS, ## 增加进度需要满足原材料，如果中途不满足将无法增加进度
	COLLECT_NEED_INGREDIENTS, ## 收获需要满足原材料，只有满足原材料时才能够进行收获，无论是否满足原材料进度都可正常增加
}

## 收获类型
enum CollectionType{
	MANUALLY, ## 手动收获，需点击已完成生产的工厂来收取产品
	AUTO, ## 自动收获，工厂生产完成时将自动收取产品
}

## 该工厂可使用的配方的ID
@export var usable_recipes: Array[StringName]

## 指定原材料从哪个空间中获取，值需为空间ID，如果留空代表该工厂实例自身所处的空间
@export var ingredients_space: StringName

## 指定原材料从哪个容器中获取，值需为容器ID，不可留空
@export var ingredients_container: StringName

## 指定产品输出到哪个空间，值需为空间ID，如果留空代表该工厂实例自身所处的空间
@export var products_space: StringName

## 指定产品输出到哪个容器，值需为容器ID，不可留空
@export var products_container: StringName

## 名称翻译标识符，即翻译表中该工厂名称的键
@export var name_key: StringName

## 图标，该工厂显示在GUI上的图标，将基于引擎设置自动生成Mipmap
@export var icon: Texture2D

## 生产类型
@export var production_type: ProductionType = ProductionType.AUTO

## 需求类型
@export var require_type: RequireType = RequireType.START_NEED_INGREDIENTS

## 收获类型
@export var collection_type: CollectionType = CollectionType.AUTO

## 设置该工厂可使用的配方的ID，用于通过代码创建实例时链式调用
func usable_recipes_(recipes: Array[StringName]) -> IdleFramework_Factory:
	usable_recipes = recipes
	return self

func make_instance() -> IdleFramework_FactoryInstance:
	var new_instance: IdleFramework_FactoryInstance = IdleFramework_FactoryInstance.new()

	return new_instance
