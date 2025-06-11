using LudeonTK;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AKBG_MainmenuBackground
{
    [StaticConstructorOnStartup]
    public class Window_BackgroundMod : Window
    {
        //      < ! - readme - ! >
        //  我把需要的接口全部都写上了
        //  同时写了一个演示用的样例
        //  对接需要把我标注的地方换成正式的数据
        //  阅读之后请删除此注释

        //cp找八云进口的ui，和本组规范不同

        //界面大小
        public override Vector2 InitialSize => new Vector2(1000.0f, 800.0f);

        //显示主体与外框之间的间隔
        protected override float Margin => 0.0f;

        public Window_BackgroundMod() : base(null)
        {
            //正下方的关闭按钮
            doCloseButton = false;
            //右上角的关闭按钮
            doCloseX = false;
            //点击外部关闭界面
            closeOnClickedOutside = false;
            //打开暂停游戏
            forcePause = true;
            //是否可拖动
            this.draggable = false;
            //是否可调整大小
            this.resizeable = false;
            //绘制主体阴影
            this.drawShadow = false;
            //是否绘制主体背景（不需要绘制泰南的背景）
            this.doWindowBackground = false;
            //层
            this.layer = WindowLayer.Super;
        }

        //// < ! - 贴图 - ! >
        //主体 - 背景
        public static readonly Texture2D Texture_Main_Background                            = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Main_Background", true);
        //主体 - 关闭
        public static readonly Texture2D Texture_Main_Button_Close                          = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Main_Button_Exit", true);
        public static readonly Texture2D Texture_Main_Button_Close_Hover                    = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Main_Button_Exit_Hover", true);
        //按钮 - 启用
        public static readonly Texture2D Texture_MainBody_Button_Apply                      = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_MainBody_Button_Apply", true);
        public static readonly Texture2D Texture_MainBody_Button_Apply_Selected             = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_MainBody_Button_Apply_Selected", true);
        //按钮 - 保存
        public static readonly Texture2D Texture_MainBody_Button_Save                       = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_MainBody_Button_Save", true);
        public static readonly Texture2D Texture_MainBody_Button_Save_Hover                 = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_MainBody_Button_Save_Hover", true);
        //标签 - 背景
        public static readonly Texture2D Texture_TabView_Icon_Back                          = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_TabView_Icon_Back", true);
        public static readonly Texture2D Texture_TabView_Icon_Back_Selected                 = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_TabView_Icon_Back_Selected", true);
        //标签 - 加载
        public static readonly Texture2D Texture_TabView_Icon_Load                          = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_TabView_Icon_Load", true);
        public static readonly Texture2D Texture_TabView_Icon_Load_Selected                 = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_TabView_Icon_Load_Selected", true);
        //随机 - 标题
        public static readonly Texture2D Texture_RandomPlay_Title                           = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_RandomPlay_Title", true);
        public static readonly Texture2D Texture_RandomPlay_Title_Hover                     = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_RandomPlay_Title_Hover", true);
        public static readonly Texture2D Texture_RandomPlay_Title_Selected                  = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_RandomPlay_Title_Selected", true);
        public static readonly Texture2D Texture_RandomPlay_Title_Selected_Hover            = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_RandomPlay_Title_Selected_Hover", true);
        //顺序 - 标题
        public static readonly Texture2D Texture_SequentialPlay_Title                       = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_SequentialPlay_Title", true);
        public static readonly Texture2D Texture_SequentialPlay_Title_Hover                 = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_SequentialPlay_Title_Hover", true);
        public static readonly Texture2D Texture_SequentialPlay_Title_Selected              = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_SequentialPlay_Title_Selected", true);
        public static readonly Texture2D Texture_SequentialPlay_Title_Selected_Hover        = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_SequentialPlay_Title_Selected_Hover", true);
        //图片 - 滑条 - 背景
        public static readonly Texture2D Texture_Image_Slider_Background                    = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Image_Slider_Background", true);
        //图片 - 滑条 - 按钮
        public static readonly Texture2D Texture_Image_Slider_DragButton                    = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Image_Slider_DragButton", true);
        //图片 - 外框
        public static readonly Texture2D Texture_Image_Out                                  = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Image_Out", true);
        public static readonly Texture2D Texture_Image_Out_Selected                         = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Image_Out_Selected", true);
        //图片 - 空位
        public static readonly Texture2D Texture_Image_Empty                                = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Image_Empty", true);
        //图片 - 添加
        public static readonly Texture2D Texture_Image_Add                                  = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Image_Add", true);
        //图片 - 删除
        public static readonly Texture2D Texture_Image_Del                                  = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Image_Del", true);
        //通用 - 秒数
        public static readonly Texture2D Texture_Play_Sec                                   = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Play_Sec", true);
        public static readonly Texture2D Texture_Play_Sec_Hover                             = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Play_Sec_Hover", true);
        public static readonly Texture2D Texture_Play_Sec_Selected                          = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Play_Sec_Selected", true);
        public static readonly Texture2D Texture_Play_Sec_Selected_Hover                    = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Play_Sec_Selected_Hover", true);
        //通用 - 滑条
        public static readonly Texture2D Texture_Play_Slider_Background                     = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Play_Slider_Background", true);
        //通用 - 滑条 - 按钮
        public static readonly Texture2D Texture_Play_Slider_DragButton                     = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Play_Slider_DragButton", true);
        public static readonly Texture2D Texture_Play_Slider_DragButton_Selected            = ContentFinder<Texture2D>.Get("Axolotl_Test/Texture_Play_Slider_DragButton_Selected", true);


        //// < ! - 颜色 - ! >
        public static readonly Color Color_Txt = new Color(0.969f, 0.576f, 0.118f);

        //// < ! - 变量 - ! >
        //选择的Tabview的Index
        public int _selectedTabIndex = 0;
        //当前承担的setting
        public BackgroundMod_Tableview_Setting _currentSetting;
        //当前选择的Image的Index
        private int _selectedImageIndex = 0;
        //是否滑动滑块
        private bool _draggingBar_Random = false;
        private bool _draggingBar_Sequential = false;
        //区分滑块
        private float _draggingBarValue_Random = 0.0f;
        private float _draggingBarValue_Sequential = 0.0f;
        //图片显示的滚动条
        private Vector2 _scrollPosition_Image = Vector2.zero;

        public override void PreOpen()
        {
            base.PreOpen();

            //获取一遍数据
            Update_currentSetting(GetTableViews().ElementAt(_selectedTabIndex).TabSetting);
        }

        public override void DoWindowContents(Rect inRect)
        {
            ////允许绘制的范围
            Rect R_Main = inRect;
            {
                //绘制主体背景
                GUI.DrawTexture(R_Main, Texture_Main_Background);

                ////主要显示部分
                Rect R_MainBodyPart = R_Main.ContractedBy(10.0f);
                {
                    ////上半部分
                    Rect R_MainBody_UpPart = new Rect()
                    {
                        x = R_MainBodyPart.x,
                        y = R_MainBodyPart.y,
                        width = R_MainBodyPart.width,
                        height = R_MainBodyPart.height / 9
                    };
                    {
                        ////关闭按钮
                        //关闭按钮的边长
                        float float_Button_Close_Side = 70.0f;
                        Rect R_MainBodyPart_Button_Close = new Rect()
                        {
                            x = R_MainBody_UpPart.x + R_MainBody_UpPart.width - float_Button_Close_Side,
                            y = R_MainBody_UpPart.y,
                            width = float_Button_Close_Side,
                            height = float_Button_Close_Side
                        };
                        {
                            if (Mouse.IsOver(R_MainBodyPart_Button_Close))
                            {
                                GUI.DrawTexture(R_MainBodyPart_Button_Close, Texture_Main_Button_Close_Hover);
                            }
                            else
                            {
                                GUI.DrawTexture(R_MainBodyPart_Button_Close, Texture_Main_Button_Close);
                            }
                            if (Widgets.ButtonInvisible(R_MainBodyPart_Button_Close, true))
                            {
                                //点击关闭按钮执行
                                this.Close(true);
                            }
                        }

                        ////上方tabview
                        //tabview的长宽
                        Vector2 V2_Tabview = new Vector2(200.0f, 40.0f);
                        Rect R_MainBodyPart_Tabview = new Rect()
                        {
                            x = R_MainBody_UpPart.x + 30.0f /* 手动偏移量 */,
                            y = R_MainBody_UpPart.y + 20.0f /* 手动偏移量 */,
                            width = R_MainBody_UpPart.x + R_MainBody_UpPart.width - R_MainBodyPart_Button_Close.width,
                            height = V2_Tabview.y - 20.0f /* 补偿手动偏移量 */
                        };
                        {
                            // < ! - 如果无数据或数据错误将不会绘制 - ! >
                            IEnumerable<BackgroundMod_Tableview> TableViews = GetTableViews();
                            for (int i = 0; i < TableViews.Count(); i++)
                            {
                                //获取数据
                                BackgroundMod_Tableview TableView = TableViews.ElementAt(i);

                                //绘制界面
                                Rect R_Tab = new Rect()
                                {
                                    x = R_MainBodyPart_Tabview.x + (i * V2_Tabview.x),
                                    y = R_MainBodyPart_Tabview.y,
                                    width = V2_Tabview.x,
                                    height = V2_Tabview.y
                                };
                                {
                                    //Icon
                                    Rect R_Tab_Icon = new Rect()
                                    {
                                        x = R_Tab.x,
                                        y = R_Tab.y,
                                        width = R_Tab.height + 10.0f /* 手动补偿量 */,
                                        height = R_Tab.height
                                    };
                                    //文本
                                    Rect R_Tab_Txt = new Rect()
                                    {
                                        x = R_Tab.x + R_Tab_Icon.width,
                                        y = R_Tab.y,
                                        width = R_Tab.width - R_Tab_Icon.width,
                                        height = R_Tab.height
                                    };

                                    //选中 与 hover 效果
                                    if (_selectedTabIndex == i || Mouse.IsOver(R_Tab))
                                    {
                                        GUI.DrawTexture(R_Tab_Icon, TableView.Texture_TabIcon_Selected);

                                        GUI.color = Color_Txt;
                                        Text.Font = GameFont.Medium;
                                        Text.Anchor = TextAnchor.MiddleCenter;
                                    }
                                    else
                                    {
                                        GUI.DrawTexture(R_Tab_Icon, TableView.Texture_TabIcon);

                                        Text.Font = GameFont.Medium;
                                        Text.Anchor = TextAnchor.MiddleCenter;
                                    }
                                    Widgets.Label(R_Tab_Txt, TableView.TabName);
                                    ResestTextFont();

                                    //点击
                                    if (Widgets.ButtonInvisible(R_Tab, true))
                                    {
                                        if(_selectedTabIndex != i)
                                        {
                                            _selectedTabIndex = i;

                                            Update_currentSetting(GetTableViews().ElementAt(i).TabSetting);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //下半部分
                    Rect R_MainBody_DownPart = new Rect()
                    {
                        x = R_MainBodyPart.x,
                        y = R_MainBodyPart.y + R_MainBody_UpPart.height,
                        width = R_MainBodyPart.width,
                        height = R_MainBodyPart.height - R_MainBody_UpPart.height
                    };
                    {
                        //左半部分
                        Rect R_DownPart_LeftPart = new Rect()
                        {
                            x = R_MainBody_DownPart.x,
                            y = R_MainBody_DownPart.y,
                            width = R_MainBody_DownPart.width / 4,
                            height = R_MainBody_DownPart.height
                        };
                        {
                            ////是否启动
                            //按钮的高度
                            float float_Button_IsApply_Height = 60.0f;
                            Rect R_DownLeftPart_Button_Apply = new Rect()
                            {
                                x = R_DownPart_LeftPart.x + 20.0f /* 手动补偿量 */,
                                y = R_DownPart_LeftPart.y + 20.0f,
                                width = R_DownPart_LeftPart.width - 40.0f /* 手动补偿量 */,
                                height = float_Button_IsApply_Height
                            };
                            {
                                if(_currentSetting.enable)
                                {
                                    GUI.DrawTexture(R_DownLeftPart_Button_Apply, Texture_MainBody_Button_Apply_Selected);
                                }
                                else
                                {
                                    GUI.DrawTexture(R_DownLeftPart_Button_Apply, Texture_MainBody_Button_Apply);
                                }

                                //文本
                                Rect R_DownLeftPart_Txt_Apply = new Rect(R_DownLeftPart_Button_Apply)
                                {
                                    y = R_DownLeftPart_Button_Apply.y - 8.0f /* 手动补偿量 */
                                };

                                Text.Font = GameFont.Medium;
                                Text.Anchor = TextAnchor.MiddleCenter;
                                Widgets.Label(R_DownLeftPart_Txt_Apply, "启用".Colorize(_currentSetting.enable ? Color_Txt : Color.white) + "/" + "关闭".Colorize(_currentSetting.enable ? Color.white : Color_Txt));

                                ResestTextFont();

                                //点击事件
                                if(Widgets.ButtonInvisible(R_DownLeftPart_Button_Apply, true))
                                {
                                    _currentSetting.enable = !_currentSetting.enable;
                                }
                            }

                            //小块之间的间隔
                            float float_Space_Between = 20.0f;
                            //拖动条的高度
                            float float_Slider_Height = 50.0f;

                            ////随机播放部分
                            Rect R_DownLeftPart_RandomPlayPart = new Rect(R_DownLeftPart_Button_Apply)
                            {
                                y = R_DownLeftPart_Button_Apply.y + R_DownLeftPart_Button_Apply.height + float_Space_Between,
                                height = R_DownLeftPart_Button_Apply.height * 1.25f + float_Slider_Height
                            };
                            {
                                //随机播放
                                Rect R_RandomPlay_UpPart = new Rect()
                                {
                                    x = R_DownLeftPart_RandomPlayPart.x,
                                    y = R_DownLeftPart_RandomPlayPart.y,
                                    width = R_DownLeftPart_RandomPlayPart.width,
                                    height = R_DownLeftPart_RandomPlayPart.height - float_Slider_Height
                                };
                                {
                                    //上半部分
                                    Rect R_UpPart = new Rect()
                                    {
                                        x = R_RandomPlay_UpPart.x,
                                        y = R_RandomPlay_UpPart.y,
                                        width = R_RandomPlay_UpPart.width,
                                        height = R_RandomPlay_UpPart.height / 10 * 6
                                    };
                                    {
                                        if (_currentSetting.randomPlay)
                                        {
                                            if (Mouse.IsOver(R_RandomPlay_UpPart))
                                            {
                                                GUI.DrawTexture(R_UpPart, Texture_RandomPlay_Title_Selected_Hover);
                                            }
                                            else
                                            {
                                                GUI.DrawTexture(R_UpPart, Texture_RandomPlay_Title_Selected);
                                            }
                                            GUI.color = Color_Txt;
                                            Text.Font = GameFont.Medium;
                                            Text.Anchor = TextAnchor.MiddleCenter;
                                        }
                                        else
                                        {
                                            if (Mouse.IsOver(R_RandomPlay_UpPart))
                                            {
                                                GUI.DrawTexture(R_UpPart, Texture_RandomPlay_Title_Hover);
                                            }
                                            else
                                            {
                                                GUI.DrawTexture(R_UpPart, Texture_RandomPlay_Title);
                                            }
                                            GUI.color = Color.white;
                                            Text.Font = GameFont.Medium;
                                            Text.Anchor = TextAnchor.MiddleCenter;
                                        }
                                        //文本
                                        Widgets.Label(R_UpPart, "随机播放");
                                        ResestTextFont();
                                    }

                                    //下半部分
                                    Rect R_DownPart = new Rect()
                                    {
                                        x = R_RandomPlay_UpPart.x,
                                        y = R_RandomPlay_UpPart.y + R_UpPart.height + 5.0f /* 手动补偿量 */,
                                        width = R_RandomPlay_UpPart.width,
                                        height = R_RandomPlay_UpPart.height - R_UpPart.height - 5.0f /* 手动补偿量 */
                                    };
                                    {
                                        if (_currentSetting.randomPlay)
                                        {
                                            if (Mouse.IsOver(R_RandomPlay_UpPart))
                                            {
                                                GUI.DrawTexture(R_DownPart, Texture_Play_Sec_Selected_Hover);
                                            }
                                            else
                                            {
                                                GUI.DrawTexture(R_DownPart, Texture_Play_Sec_Selected);
                                            }
                                            GUI.color = Color_Txt;
                                            Text.Font = GameFont.Small;
                                            Text.Anchor = TextAnchor.MiddleLeft;
                                        }
                                        else
                                        {
                                            if (Mouse.IsOver(R_RandomPlay_UpPart))
                                            {
                                                GUI.DrawTexture(R_DownPart, Texture_Play_Sec_Hover);
                                            }
                                            else
                                            {
                                                GUI.DrawTexture(R_DownPart, Texture_Play_Sec);
                                            }
                                            GUI.color = Color.white;
                                            Text.Font = GameFont.Small;
                                            Text.Anchor = TextAnchor.MiddleLeft;
                                        }
                                        //文本
                                        Rect R_DownPart_Txt = new Rect(R_DownPart)
                                        {
                                            x = R_DownPart.x + 10.0f /* 手动补偿量 */,
                                            width = R_DownPart.width - 10.0f /* 手动补偿量 */,
                                        };
                                        Widgets.Label(R_DownPart_Txt, "随机时间间隔");
                                        ResestTextFont();
                                    }

                                    //点击事件
                                    if(Widgets.ButtonInvisible(R_RandomPlay_UpPart, true))
                                    {
                                        _currentSetting.randomPlay = true;
                                    }
                                }

                                //时间滚动条
                                Rect R_RandomPlay_Slider = new Rect()
                                {
                                    x = R_DownLeftPart_RandomPlayPart.x,
                                    y = R_DownLeftPart_RandomPlayPart.y + R_RandomPlay_UpPart.height + 5.0f /* 手动偏移量 */,
                                    width = R_DownLeftPart_RandomPlayPart.width,
                                    height = float_Slider_Height
                                };
                                {
                                    //显示的刻度
                                    Rect R_Slider_Background = new Rect(R_RandomPlay_Slider)
                                    {
                                        y = R_RandomPlay_Slider.y + 20.0f /* 手动补偿量 */,
                                        height = R_RandomPlay_Slider.height - 20.0f /* 手动补偿量 */
                                    };
                                    {
                                        GUI.DrawTexture(R_Slider_Background, Texture_Play_Slider_Background);
                                    }

                                    //滑块
                                    Rect R_Slider_Button = new Rect(R_RandomPlay_Slider)
                                    {
                                        height = R_RandomPlay_Slider.height - 20.0f /* 手动补偿量 */,
                                    };
                                    {
                                        DraggableBar
                                        (
                                            R_Slider_Button,
                                            Mouse.IsOver(R_Slider_Button)
                                                ? Texture_Play_Slider_DragButton_Selected
                                                : Texture_Play_Slider_DragButton,
                                            ref _draggingBar_Random,
                                            ref _draggingBarValue_Random,
                                            50
                                        );
                                    }
                                }
                            }

                            //按顺序播放
                            Rect R_DownLeftPart_SequentialPlayPart = new Rect(R_DownLeftPart_RandomPlayPart)
                            {
                                y = R_DownLeftPart_RandomPlayPart.y + R_DownLeftPart_RandomPlayPart.height + float_Space_Between,
                            };
                            {
                                //顺序播放
                                Rect R_SequentialPlay_UpPart = new Rect()
                                {
                                    x = R_DownLeftPart_SequentialPlayPart.x,
                                    y = R_DownLeftPart_SequentialPlayPart.y,
                                    width = R_DownLeftPart_SequentialPlayPart.width,
                                    height = R_DownLeftPart_SequentialPlayPart.height - float_Slider_Height
                                };
                                {
                                    //上半部分
                                    Rect R_UpPart = new Rect()
                                    {
                                        x = R_SequentialPlay_UpPart.x,
                                        y = R_SequentialPlay_UpPart.y,
                                        width = R_SequentialPlay_UpPart.width,
                                        height = R_SequentialPlay_UpPart.height / 10 * 6
                                    };
                                    {
                                        if (!_currentSetting.randomPlay)
                                        {
                                            if (Mouse.IsOver(R_SequentialPlay_UpPart))
                                            {
                                                GUI.DrawTexture(R_UpPart, Texture_SequentialPlay_Title_Selected_Hover);
                                            }
                                            else
                                            {
                                                GUI.DrawTexture(R_UpPart, Texture_SequentialPlay_Title_Selected);
                                            }
                                            GUI.color = Color_Txt;
                                            Text.Font = GameFont.Medium;
                                            Text.Anchor = TextAnchor.MiddleCenter;
                                        }
                                        else
                                        {
                                            if (Mouse.IsOver(R_SequentialPlay_UpPart))
                                            {
                                                GUI.DrawTexture(R_UpPart, Texture_SequentialPlay_Title_Hover);
                                            }
                                            else
                                            {
                                                GUI.DrawTexture(R_UpPart, Texture_SequentialPlay_Title);
                                            }
                                            GUI.color = Color.white;
                                            Text.Font = GameFont.Medium;
                                            Text.Anchor = TextAnchor.MiddleCenter;
                                        }
                                        //文本
                                        Widgets.Label(R_UpPart, "按顺序播放");
                                        ResestTextFont();
                                    }

                                    //下半部分
                                    Rect R_DownPart = new Rect()
                                    {
                                        x = R_SequentialPlay_UpPart.x,
                                        y = R_SequentialPlay_UpPart.y + R_UpPart.height + 5.0f /* 手动补偿量 */,
                                        width = R_SequentialPlay_UpPart.width,
                                        height = R_SequentialPlay_UpPart.height - R_UpPart.height - 5.0f /* 手动补偿量 */
                                    };
                                    {
                                        if (!_currentSetting.randomPlay)
                                        {
                                            if (Mouse.IsOver(R_SequentialPlay_UpPart))
                                            {
                                                GUI.DrawTexture(R_DownPart, Texture_Play_Sec_Selected_Hover);
                                            }
                                            else
                                            {
                                                GUI.DrawTexture(R_DownPart, Texture_Play_Sec_Selected);
                                            }
                                            GUI.color = Color_Txt;
                                            Text.Font = GameFont.Small;
                                            Text.Anchor = TextAnchor.MiddleLeft;
                                        }
                                        else
                                        {
                                            if (Mouse.IsOver(R_SequentialPlay_UpPart))
                                            {
                                                GUI.DrawTexture(R_DownPart, Texture_Play_Sec_Hover);
                                            }
                                            else
                                            {
                                                GUI.DrawTexture(R_DownPart, Texture_Play_Sec);
                                            }
                                            GUI.color = Color.white;
                                            Text.Font = GameFont.Small;
                                            Text.Anchor = TextAnchor.MiddleLeft;
                                        }
                                        //文本
                                        Rect R_DownPart_Txt = new Rect(R_DownPart)
                                        {
                                            x = R_DownPart.x + 10.0f /* 手动补偿量 */,
                                            width = R_DownPart.width - 10.0f /* 手动补偿量 */,
                                        };
                                        Widgets.Label(R_DownPart_Txt, "播放时间间隔");
                                        ResestTextFont();
                                    }

                                    //点击事件
                                    if (Widgets.ButtonInvisible(R_SequentialPlay_UpPart, true))
                                    {
                                        _currentSetting.randomPlay = false;
                                    }
                                }

                                //时间滚动条
                                Rect R_SequentialPlay_Slider = new Rect()
                                {
                                    x = R_DownLeftPart_SequentialPlayPart.x,
                                    y = R_DownLeftPart_SequentialPlayPart.y + R_SequentialPlay_UpPart.height + 5.0f /* 手动偏移量 */,
                                    width = R_DownLeftPart_SequentialPlayPart.width,
                                    height = float_Slider_Height
                                };
                                {
                                    //显示的刻度
                                    Rect R_Slider_Background = new Rect(R_SequentialPlay_Slider)
                                    {
                                        y = R_SequentialPlay_Slider.y + 20.0f /* 手动补偿量 */,
                                        height = R_SequentialPlay_Slider.height - 20.0f /* 手动补偿量 */
                                    };
                                    {
                                        GUI.DrawTexture(R_Slider_Background, Texture_Play_Slider_Background);
                                    }

                                    //滑块
                                    Rect R_Slider_Button = new Rect(R_SequentialPlay_Slider)
                                    {
                                        height = R_SequentialPlay_Slider.height - 20.0f /* 手动补偿量 */,
                                    };
                                    {
                                        DraggableBar
                                        (
                                            R_Slider_Button,
                                            Mouse.IsOver(R_Slider_Button)
                                                ? Texture_Play_Slider_DragButton_Selected
                                                : Texture_Play_Slider_DragButton,
                                            ref _draggingBar_Sequential,
                                            ref _draggingBarValue_Sequential,
                                            50
                                        );
                                    }
                                }
                            }

                            ////保存按钮
                            //按钮的高度
                            float float_Button_Save_Height = 50.0f;
                            Rect R_DownLeftPart_Button_Save = new Rect()
                            {
                                x = R_DownPart_LeftPart.x + 20.0f /* 手动补偿量 */,
                                y = R_DownPart_LeftPart.y + R_DownPart_LeftPart.height - float_Button_Save_Height - 20.0f /* 手动偏移量 */,
                                width = R_DownPart_LeftPart.width - 40.0f /* 手动补偿量 */,
                                height = float_Button_Save_Height
                            };
                            {
                                //贴图
                                if (Mouse.IsOver(R_DownLeftPart_Button_Save))
                                {
                                    GUI.DrawTexture(R_DownLeftPart_Button_Save, Texture_MainBody_Button_Save_Hover);
                                }
                                else
                                {
                                    GUI.DrawTexture(R_DownLeftPart_Button_Save, Texture_MainBody_Button_Save);
                                }

                                //文本
                                GUI.color = Mouse.IsOver(R_DownLeftPart_Button_Save) ? Color_Txt : Color.white;
                                Text.Font = GameFont.Medium;
                                Text.Anchor = TextAnchor.MiddleCenter;
                                Widgets.Label(R_DownLeftPart_Button_Save, "保存配置");

                                ResestTextFont();


                                // < ! - 保存配置的点击事件 - ！ >
                                if (Widgets.ButtonInvisible(R_DownLeftPart_Button_Save))
                                {
                                    //操作写在这里

                                    //注：
                                    //  由于分开了两个滚动条，所以区分设置
                                    //  如果是随机播放，就使用 _draggingBarValue_Random 的值
                                    //  如果是顺序播放，就使用 _draggingBarValue_Sequential 的值
                                    //  不要因为currentSetting类型一样就直接使用currentSetting.SecondsPerImage
                                    //  滚动条单独区分导致不直接使用currentSetting.SecondsPerImage
                                }
                            }
                        }

                        //右半部分
                        Rect R_DownPart_RightPart = new Rect()
                        {
                            x = R_MainBody_DownPart.x + R_DownPart_LeftPart.width,
                            y = R_MainBody_DownPart.y,
                            width = R_MainBody_DownPart.width - R_DownPart_LeftPart.width,
                            height = R_MainBody_DownPart.height
                        };
                        {
                            //图片部分 与 滚动条部分
                            Rect R_RightPart_MainBodyPart = new Rect(R_DownPart_RightPart)
                            {
                                x = R_DownPart_RightPart.x + 20.0f /* 手动偏移量 */,
                                y = R_DownPart_RightPart.y + 10.0f /* 手动偏移量 */,
                                width = R_DownPart_RightPart.width - 20.0f /* 手动偏移量 */,
                                height = R_DownPart_RightPart.height - 30.0f /* 手动偏移量 */,
                            };
                            {
                                ////图片部分
                                //滑块部分宽度
                                float float_Slider_Image_Width = 30.0f;
                                Rect R_MainBodyPart_Image = new Rect(R_RightPart_MainBodyPart)
                                {
                                    width = R_RightPart_MainBodyPart.width - float_Slider_Image_Width,
                                    height = R_RightPart_MainBodyPart.height - 20.0f /* 手动偏移量 */,
                                };
                                {
                                    //获取数据
                                    // < ! - 这里是演示用数据，正式环境请放入真实数据 - ! >
                                    IEnumerable<Texture2D> TextureList = new List<Texture2D>
                                    {
                                        Texture_Image_Empty,
                                        Texture_Image_Empty,
                                        Texture_Image_Empty,

                                        Texture_Image_Empty,
                                        Texture_Image_Empty,
                                        Texture_Image_Empty,

                                        Texture_Image_Empty,
                                        Texture_Image_Empty,
                                        Texture_Image_Empty,

                                        Texture_Image_Empty,
                                        Texture_Image_Empty,
                                        Texture_Image_Empty,

                                        Texture_Image_Empty,
                                    };

                                    //图片之间的间隔
                                    float float_Space_Between_Image = 5.0f;
                                    //图片位置大小
                                    Vector2 V2_ImageSize = new Vector2((R_MainBodyPart_Image.width - 2 * float_Space_Between_Image) / 3, 150.0f);
                                    //实际装载图片空间的界面
                                    Rect R_ImageArea = new Rect(R_MainBodyPart_Image)
                                    {
                                        height = (TextureList.Count() / 3 + 1) * (V2_ImageSize.y + float_Space_Between_Image)
                                    };
                                    ////显示图片
                                    Widgets.BeginScrollView(R_MainBodyPart_Image, ref _scrollPosition_Image, R_ImageArea, false);
                                    {
                                        for (int i = 0; i < TextureList.Count(); i++)
                                        {
                                            Texture2D texture = TextureList.ElementAt(i);

                                            Rect R_DrawImage = new Rect()
                                            {
                                                x = R_ImageArea.x + (i % 3) * (V2_ImageSize.x + float_Space_Between_Image),
                                                y = R_ImageArea.y + (i / 3) * (V2_ImageSize.y + float_Space_Between_Image),
                                                width = V2_ImageSize.x,
                                                height = V2_ImageSize.y
                                            };
                                            {
                                                //绘制图片
                                                GUI.DrawTexture(R_DrawImage, texture);

                                                //绘制外框
                                                if(_selectedImageIndex == i)
                                                {
                                                    //选中
                                                    GUI.DrawTexture(R_DrawImage, Texture_Image_Out_Selected);

                                                    ////选中时可以进行添加删除的操作
                                                    //操作按钮边长
                                                    float float_Image_Button_Side = 30.0f;
                                                    Rect R_Image_Button = new Rect()
                                                    {
                                                        x = R_DrawImage.x + R_DrawImage.width - float_Image_Button_Side,
                                                        y = R_DrawImage.y + R_DrawImage.height - float_Image_Button_Side,
                                                        width = float_Image_Button_Side,
                                                        height = float_Image_Button_Side
                                                    };
                                                    {
                                                        // < ! - 如果这个图片被选中，就显示删除按钮 - ！ >
                                                        if (true)
                                                        {
                                                            GUI.DrawTexture(R_Image_Button, Texture_Image_Del);
                                                        }
                                                        else
                                                        {
                                                            GUI.DrawTexture(R_Image_Button, Texture_Image_Add);
                                                        }

                                                        //点击事件
                                                        if(Widgets.ButtonInvisible(R_Image_Button, true))
                                                        {
                                                            // < ! - 如果这个图片被选中，就执行删除 - ！ >
                                                            if(true)
                                                            {
                                                                //删除图片的操作
                                                            }
                                                            // < ! - 如果这个图片没有被选中，就执行添加 - ！ >
                                                            else
                                                            {
                                                                //添加图片的操作
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //未选中
                                                    GUI.DrawTexture(R_DrawImage, Texture_Image_Out);
                                                }

                                                //点击事件
                                                if (Widgets.ButtonInvisible(R_DrawImage, true))
                                                {
                                                    _selectedImageIndex = i;
                                                }
                                            }
                                            ;
                                        }
                                    }
                                    Widgets.EndScrollView();
                                }

                                ////滑块部分
                                Rect R_MainBodyPart_Slider = new Rect(R_RightPart_MainBodyPart)
                                {
                                    x = R_RightPart_MainBodyPart.x + R_RightPart_MainBodyPart.width - float_Slider_Image_Width,
                                    width = float_Slider_Image_Width,
                                };
                                {
                                    //背景
                                    Rect R_Slider_Background = new Rect(R_MainBodyPart_Slider)
                                    {
                                        x = R_MainBodyPart_Slider.x + 10.0f /* 手动补偿量 */,
                                        width = R_MainBodyPart_Slider.width - 10.0f /* 手动补偿量 */
                                    };
                                    {
                                        GUI.DrawTexture(R_Slider_Background, Texture_Image_Slider_Background);
                                    }
                                }

                                //GUI.DrawTexture(R_RightPart_MainBodyPart, SolidColorMaterials.NewSolidColorTexture(Color.red));
                            }
                        }
                    }
                }
            }
        }

        //获取Tabview
        // < ! - 这里需要放入真实数据，如果不想进行这种转换，可以重写class部分，对界面中的判断条件进行替换即可 - ! >
        public IEnumerable<BackgroundMod_Tableview> GetTableViews()
        {
            yield return new BackgroundMod_Tableview
            (
                TabName: "背景图片".Translate(),
                Texture_TabIcon: Texture_TabView_Icon_Back,
                Texture_TabIcon_Selected: Texture_TabView_Icon_Back_Selected,
                TabSetting: new BackgroundMod_Tableview_Setting
                (
                    IsApply: true,
                    IsRandomPlay: true,
                    //注：SecondsPerImage需要以0-1之间的值进行设置
                    SecondsPerImage: 0.5f,
                    TextureList: new List<Texture2D>
                    {
                        Texture_Main_Button_Close,
                    }
                ),
                TextureList_Save: new List<Texture2D>
                {
                    Texture_Main_Button_Close,
                    Texture_Main_Background,
                    Texture_Main_Button_Close,
                    Texture_Main_Background,
                    Texture_Main_Button_Close,
                    Texture_Main_Background,
                }

            );
            yield return new BackgroundMod_Tableview
            (
                TabName: "加载图片".Translate(),
                Texture_TabIcon: Texture_TabView_Icon_Load,
                Texture_TabIcon_Selected: Texture_TabView_Icon_Load_Selected,
                TabSetting: new BackgroundMod_Tableview_Setting
                (
                    IsApply: true,
                    IsRandomPlay: false,
                    SecondsPerImage: 0.8f,
                    TextureList: new List<Texture2D>
                    {
                        Texture_Main_Background,
                    }
                ),
                TextureList_Save: new List<Texture2D>
                {
                    Texture_Main_Background,
                    Texture_Main_Button_Close,
                    Texture_Main_Background,
                    Texture_Main_Button_Close,
                    Texture_Main_Background,
                    Texture_Main_Button_Close,
                }
            );
        }

        //拖动条
        public static void DraggableBar(Rect barRect, Texture2D dragBarTex,ref bool draggingBar, ref float targetValue, int increments = 20, float min = 0f, float max = 1f)
        {
            bool flag = Mouse.IsOver(barRect);
            float num = Mathf.Clamp(Mathf.Round((Event.current.mousePosition.x - barRect.x) / barRect.width * (float)increments) / (float)increments, min, max);
            Event current = Event.current;
            if (current.type == EventType.MouseDown && current.button == 0 && flag)
            {
                targetValue = num;
                draggingBar = true;
                current.Use();
            }
            if ((UnityGUIBugsFixer.MouseDrag(0) & draggingBar) && flag)
            {
                if (Math.Abs(num - targetValue) > 1E-45f)
                {
                    SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                }
                targetValue = num;
                if (Event.current.type == EventType.MouseDrag)
                {
                    current.Use();
                }
            }
            if ((current.type == EventType.MouseUp && current.button == 0) & draggingBar)
            {
                draggingBar = false;
                current.Use();
            }
            DrawDraggableBarTarget(barRect, draggingBar ? num : targetValue, dragBarTex);
            GUI.color = Color.white;
        }

        private static void DrawDraggableBarTarget(Rect rect, float percent, Texture2D targetTex)
        {
            float num = Mathf.Round((rect.width - 8f) * percent);
            GUI.DrawTexture(new Rect
            {
                x = rect.x + num,
                y = rect.y,
                width = 11.0f,
                height = rect.height
            }, targetTex);
        }

        public void Update_currentSetting(BackgroundMod_Tableview_Setting setting)
        {
            _currentSetting = setting;

            if (_currentSetting.randomPlay)
            {
                _draggingBarValue_Random = _currentSetting.randomPlayInterval;
                _draggingBarValue_Sequential = 0.0f;
            }
            else
            {
                _draggingBarValue_Random = 0.0f;
                _draggingBarValue_Sequential = _currentSetting.randomPlayInterval;
            }
            _scrollPosition_Image = Vector2.zero;
            _selectedImageIndex = 0;
        }

        //刷新画笔
        public static void ResestTextFont()
        {
            //重置
            GUI.color = Color.white;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.UpperLeft;
        }
    }

    public class BackgroundMod_Tableview
    {
        [MustTranslate]
        [DefaultValue(null)]
        [Description("界面名称")]
        public string TabName;

        [DefaultValue(null)]
        [Description("界面图标")]
        public Texture2D Texture_TabIcon;

        [DefaultValue(null)]
        [Description("界面图标（选中）")]
        public Texture2D Texture_TabIcon_Selected;

        [Description("这个界面的设置")]
        public BackgroundMod_Tableview_Setting TabSetting;

        [Description("该界面所有的显示图片")]
        public List<Texture2D> TextureList_Save;

        public BackgroundMod_Tableview(string TabName, Texture2D Texture_TabIcon, Texture2D Texture_TabIcon_Selected, BackgroundMod_Tableview_Setting TabSetting, List<Texture2D> TextureList_Save = null)
        {
            this.TabName = TabName;
            this.Texture_TabIcon = Texture_TabIcon;
            this.Texture_TabIcon_Selected = Texture_TabIcon_Selected;
            this.TabSetting = TabSetting;
            this.TextureList_Save = TextureList_Save ?? new List<Texture2D>();
        }
    }


}
