using System.Collections.Generic;

#nullable enable
public static class AthanaEvents
{

    /// <summary>
    /// 退出登录事件
    /// </summary>
    /// <param name="extra"></param>
    public static void Logout(Dictionary<string, object>? extra = null)
    {
        AthanaSdk.SendEvent("Logout", paramMap: extra);
    }

    /// <summary>
    /// 记录游戏任务事件
    /// </summary>
    /// 
    /// <param name="taskId">任务ID</param>
    /// <param name="taskType">任务类型：1 - 一次性任务；2 - 重复任务</param>
    /// <param name="repeatCycle">重复周期，是重复任务的前提下可传秒钟为单位，传int数字，每日重复，则传 86400</param>
    /// <param name="repeatCycleTimes">期望数量</param>
    /// <param name="extra">拓展参数，只支持基础变量类型</param>
    public static void LogGamesTask(
        int taskId,
        int taskType,
        int? repeatCycle = null,
        int? repeatCycleTimes = null,
        Dictionary<string, object>? extra = null)
    {
        var paramMap = new Dictionary<string, object>();
        paramMap["task_id"] = taskId;
        paramMap["task_type"] = taskType;
        if (repeatCycle != null)
        {
            paramMap["repeat_cycle"] = repeatCycle;
        }
        if (repeatCycleTimes != null)
        {
            paramMap["repeat_cycle_tims"] = repeatCycleTimes;
        }
        if (extra != null)
        {
            foreach (var item in extra)
            {
                paramMap[item.Key] = item.Value;
            }
        }
        AthanaSdk.SendEvent("Task", paramMap: paramMap);
    }

    /// <summary>
    /// 游戏通关成功事件
    /// </summary>
    /// <param name="clearCostTime">通关消耗时长，精确到毫秒</param>
    /// <param name="level">通关关卡，字符串，需要区分小关的话，可以用 "1-10"、"1-11"等</param>
    /// <param name="score">通过分数</param>
    /// <param name="clearTimes">累计通关关卡次数</param>
    /// <param name="isRecharge">是否在关卡中充值，1-是；2-否</param>
    /// <param name="rechargeAmount">充值金额,如果有充值，可传,最好以 美元/美分为单位</param>
    /// <param name="currency">如果广告商返回非 美元/美分 的预估收益，需要将 货币单位 返回来</param>
    /// <param name="paid">是否收费关卡，1-是；2-否</param>
    /// <param name="extra">拓展参数，只支持基础变量类型</param>
    public static void LogGamesStageFinished(
        int clearCostTime,
        string level,
        int? score = null,
        int? clearTimes = null,
        int? isRecharge = null,
        double? rechargeAmount = null,
        string? currency = null,
        int? paid = null,
        Dictionary<string, object>? extra = null)
    {
        var paramMap = new Dictionary<string, object>();
        paramMap["clear_cost_time"] = clearCostTime;
        paramMap["level"] = level;
        if (score != null)
        {
            paramMap["score"] = score;
        }
        if (clearTimes != null)
        {
            paramMap["clear_times"] = clearTimes;
        }
        if (isRecharge != null)
        {
            paramMap["is_recharge"] = isRecharge;
        }
        if (rechargeAmount != null)
        {
            paramMap["recharge_amount"] = rechargeAmount;
        }
        if (currency != null)
        {
            paramMap["currency"] = currency;
        }
        if (paid != null)
        {
            paramMap["is_vip"] = paid;
        }
        if (extra != null)
        {
            foreach (var item in extra)
            {
                paramMap[item.Key] = item.Value;
            }
        }
        AthanaSdk.SendEvent("ClearRecord", paramMap: paramMap);
    }

    /// <summary>
    /// 游戏通关失败事件
    /// </summary>
    /// <param name="clearCostTime">通关消耗时长，精确到毫秒</param>
    /// <param name="level">当前关卡，字符串，需要区分小关的话，可以用 "1-10"、"1-11"等</param>
    /// <param name="levelType">关卡类型：0 - 普通关卡，1 - 副本关卡，2 - 日常关卡</param>
    /// <param name="reason">失败原因：1 - 超过限制时间；2 - 没有可移动的步骤</param>
    /// <param name="paid">是否收费关卡，1-是；2-否</param>
    /// <param name="extra">拓展参数，只支持基础变量类型</param>
    public static void LogGamesStageNotFinish(
        int clearCostTime,
        string level,
        int? levelType = null,
        int? reason = null,
        int? paid = null,
        Dictionary<string, object>? extra = null)
    {
        var paramMap = new Dictionary<string, object>();
        paramMap["clear_cost_time"] = clearCostTime;
        paramMap["level"] = level;
        if (levelType != null)
        {
            paramMap["level_type"] = levelType;
        }
        if (reason != null)
        {
            paramMap["reason"] = reason;
        }
        if (paid != null)
        {
            paramMap["is_vip"] = paid;
        }
        if (extra != null)
        {
            foreach (var item in extra)
            {
                paramMap[item.Key] = item.Value;
            }
        }
        AthanaSdk.SendEvent("ClearFailRecord", paramMap: paramMap);
    }

    /// <summary>
    /// 新手教程完成事件
    /// </summary>
    /// <param name="costTime">消耗时长，精确到毫秒</param>
    /// <param name="startTrialId">新手教程id</param>
    /// <param name="startTrialName">新手教程名称</param>
    /// <param name="extra">拓展参数，只支持基础变量类型</param>
    public static void LogGamesStartTrialFinished(
        int costTime,
        int startTrialId,
        string? startTrialName = null,
        Dictionary<string, object>? extra = null)
    {
        var paramMap = new Dictionary<string, object>();
        paramMap["is_succ"] = 1;
        paramMap["cost_time"] = costTime;
        paramMap["start_trial_id"] = startTrialId;
        if (startTrialName != null)
        {
            paramMap["start_trial_name"] = startTrialName;
        }
        if (extra != null)
        {
            foreach (var item in extra)
            {
                paramMap[item.Key] = item.Value;
            }
        }
        AthanaSdk.SendEvent("StartTrial", paramMap: paramMap);
    }

    /// <summary>
    /// 新手教程未完成（跳过）事件
    /// </summary>
    /// <param name="costTime">消耗时长，精确到毫秒</param>
    /// <param name="startTrialId">新手教程id</param>
    /// <param name="startTrialName">新手教程名称</param>
    /// <param name="extra">拓展参数，只支持基础变量类型</param>
    public static void LogGamesStartTrialNotFinish(
        int costTime,
        int startTrialId,
        string? startTrialName = null,
        Dictionary<string, object>? extra = null)
    {
        var paramMap = new Dictionary<string, object>();
        paramMap["is_succ"] = 2;
        paramMap["cost_time"] = costTime;
        paramMap["start_trial_id"] = startTrialId;
        if (startTrialName != null)
        {
            paramMap["start_trial_name"] = startTrialName;
        }
        if (extra != null)
        {
            foreach (var item in extra)
            {
                paramMap[item.Key] = item.Value;
            }
        }
        AthanaSdk.SendEvent("StartTrial", paramMap: paramMap);
    }

    /// <summary>
    /// 游戏角色升级/关卡通过事件
    /// </summary>
    /// <param name="oldLevel">旧等级 / 关卡</param>
    /// <param name="newLevel">新等级 / 关卡</param>
    /// <param name="scene">发生场景：1 - 通关关卡（副本）；2 - 做任务；3 - 击杀小怪</param>
    /// <param name="sceneExt">场景额外参数，例如：第几关卡、任务ID</param>
    /// <param name="roleId">游戏角色Id，适用于多角色类游戏</param>
    /// <param name="extra">拓展参数，只支持基础变量类型</param>
    public static void LogGamesLevelUp(
        int oldLevel,
        int newLevel,
        int? scene = null,
        string? sceneExt = null,
        int? roleId = null,
        Dictionary<string, object>? extra = null)
    {
        var paramMap = new Dictionary<string, object>();
        paramMap["old_level"] = oldLevel;
        paramMap["new_level"] = newLevel;
        if (scene != null)
        {
            paramMap["scene"] = scene;
        }
        if (sceneExt != null)
        {
            paramMap["scene_ext"] = sceneExt;
        }
        if (roleId != null)
        {
            paramMap["role_id"] = roleId;
        }
        if (extra != null)
        {
            foreach (var item in extra)
            {
                paramMap[item.Key] = item.Value;
            }
        }
        AthanaSdk.SendEvent("LevelAchieved", paramMap: paramMap);
    }

    /// <summary>
    /// 记录分享事件
    /// </summary>
    /// <param name="extra">拓展参数，只支持基础变量类型</param>
    public static void LogShare(Dictionary<string, object>? extra = null)
    {
        AthanaSdk.SendEvent("Share", paramMap: extra);
    }

    /// <summary>
    /// 记录邀请事件
    /// </summary>
    /// <param name="extra">拓展参数，只支持基础变量类型</param>
    public static void LogInvite(Dictionary<string, object>? extra = null)
    {
        AthanaSdk.SendEvent("Invite", paramMap: extra);
    }
}