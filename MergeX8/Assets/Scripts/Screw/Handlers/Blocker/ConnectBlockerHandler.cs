using System.Collections.Generic;

namespace Screw
{
    public class ConnectBlockerHandler : BlockerHandler
    {
        List<ScrewBlocker> _ignore = new List<ScrewBlocker>() {ScrewBlocker.ConnectBlocker};

        public ConnectBlockerHandler(ScrewGameContext inContext) : base(inContext)
        {
        }

        public override bool IsScrewDownValid(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker && screwModel.ScrewBlockers.TryGetValue(ScrewBlocker.ConnectBlocker, out BaseBlockerModel model))
            {
                if (model is ConnectBlockerModel connectBlockerModel)
                {
                    if (connectBlockerModel.IsComplete())
                    {
                        return true;
                    }

                    // TODO 这里要检测判断一下连着的钉子是否符合blocker
                    var list = new List<ScrewModel>();
                    for (int i = 0; i < connectBlockerModel.ConnectScrew.Count; i++)
                    {
                        foreach (var layerModel in context.LevelModel.LayerModels.Values)
                        {
                            if (layerModel.ScrewModels.ContainsKey(connectBlockerModel.ConnectScrew[i]))
                            {
                                list.Add(layerModel.ScrewModels[connectBlockerModel.ConnectScrew[i]]);
                            }
                        }
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (!context.blockersHandler.IsScrewDownValid(list[i], _ignore))
                        {
                            return false;
                        }
                    }
                    
                    if (connectBlockerModel.IsBlocking())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool CompleteBlockerInRevive(ScrewModel model)
        {
            return true;
        }

        public override bool HandleScrewUp(ScrewModel screwModel, ScrewView view)
        {
            if (!IsScrewDownValid(screwModel))
            {
                SoundModule.PlaySfx("sfx_obstacle_rope_2");
                view.DoJump(); 
            }

            return false;
        }

        public override bool HandleScrewDown(ScrewModel model, ScrewView view)
        {
            if (model.HasBlocker
                && model.ScrewBlockers.TryGetValue(ScrewBlocker.ConnectBlocker, out BaseBlockerModel baseModel))
            {
                if (baseModel is ConnectBlockerModel connectBlockerModel)
                {
                    var list = new List<ScrewModel>();
                    var views = new List<ScrewView>();

                    // 优先移动点击的螺丝
                    list.Add(model);

                    for (int i = 0; i < connectBlockerModel.ConnectScrew.Count; i++)
                    {
                        foreach (var layerModel in context.LevelModel.LayerModels.Values)
                        {
                            if (layerModel.ScrewModels.ContainsKey(connectBlockerModel.ConnectScrew[i]))
                            {
                                list.Add(layerModel.ScrewModels[connectBlockerModel.ConnectScrew[i]]);
                            }
                        }
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        views.Add(context.GetScrewView(list[i]));
                    }

                    SoundModule.PlaySfx("sfx_obstacle_rope_1");
                    context.actionController.AddPairMoveAction(list, views).Forget();

                    return true;
                }
            }
            
            return false;
        }
    }
}