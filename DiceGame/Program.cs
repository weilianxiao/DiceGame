public class Program
{
    /// <summary>
    /// 单价
    /// </summary>
    private static int perCountCost = 10;
    /// <summary>
    /// 分割线
    /// </summary>
    private static string splitLine = "--------------------------------------------------------------------";
    /// <summary>
    /// 画板数组
    /// </summary>
    public static List<DiceModel> list = new()
    {
        new () { Number = 11, Amount = 200 },
        new () { Number = 18, Amount = 1000 },
        new () { Number = 9, Amount = 500 },
        new () { Number = 20, Amount = 1400 },
        new () { Number = 7, Amount = 100 },
        new () { Number = 22, Amount = 1000 },
        new () { Number = 5, Amount = 400 },
        new () { Number = 24, Amount = 1400 },
        new () { Number = 29, Amount = 300 },
        new () { Number = 26, Amount = 1200 },
        new () { Number = 27, Amount = -580 },
        new () { Number = 28, Amount = 1600 },
        new () { Number = 25, Amount = 300 },
        new () { Number = 30, Amount = 6000 },
        new () { Number = 23, Amount = 200 },
        new () { Number = 6, Amount = 1400 },
        new () { Number = 21, Amount = 300 },
        new () { Number = 8, Amount = 1200 },
        new () { Number = 19, Amount = 300 },
        new () { Number = 10, Amount = 1000 },
        new () { Number = 17, Amount = 400 },
        new () { Number = 12, Amount = 1200 },
        new () { Number = 15, Amount = 200 },
        new () { Number = 14, Amount = 1000 },
        new () { Number = 13, Amount = 600 },
        new () { Number = 16, Amount = 1200 },
    };
    /// <summary>
    /// 反转画板数组
    /// </summary>
    public static List<DiceModel> reversedList = list.ReverseCopy();
    /// <summary>
    /// 用户战绩
    /// </summary>
    public static List<UserDiceModel> userList = new();

    public static void Main(string[] args)
    {
        PrintPaint();
        Console.WriteLine("欢迎来到大富翁摇骰子游戏!");
        Console.WriteLine(@"游戏规则:
1.先选择方向,顺时针or逆时针
2.五颗骰子一起丢,把骰子的点数加起来,是多少就从多少开始数,数跟骰子总和相同的步数
3.假设选择了顺时针方向,点数总和为18,在盘面上找到18作为起点,从18点开始数,数18步
4.豹子直接拿钱,1/2豹子2000元,3/4豹子4000元,5/6豹子6000元");

        Console.WriteLine(splitLine);

    retryChooseState:
        Console.WriteLine("");
        Console.WriteLine("游戏开始");
        Console.WriteLine("请选择方向,1:顺时针方向,2:逆时针方向");
        var stateInput = Console.ReadLine();
        if (stateInput != "1" && stateInput != "2")
        {
            Console.WriteLine("输入有误,请重新输入");
            goto retryChooseState;
        }
        Console.WriteLine("请输入投掷次数");
    retryChooseCount:
        var countStr = Console.ReadLine();
        if (!int.TryParse(countStr, out int count) || count < 1 || count > 10000)
        {
            Console.WriteLine("输入有误,请输入介于1~10000的正整数");
            goto retryChooseCount;
        }
        var state = stateInput == "1" ? DirectionType.ClockWise : DirectionType.CounterClockWise;

        for (int i = 0; i < count; i++)
        {
            var random = new Random();
            var value1 = random.Next(1, 6);
            var value2 = random.Next(1, 6);
            var value3 = random.Next(1, 6);
            var value4 = random.Next(1, 6);
            var value5 = random.Next(1, 6);
            var values = new List<int>
            {
                value1,
                value2,
                value3,
                value4,
                value5
            };
            MoveNext(state, values);
        }
        Console.WriteLine(splitLine);
        var cost = count * perCountCost;
        var money = userList.Sum(x => x.Amount);
        var total = money - cost;
        var winCount = userList.Count(x => x.Amount > 0);
        var loseCount = userList.Count(x => x.Amount < 0);
        var winRate = Math.Round((double)winCount / count * 100, 2);
        var loseRate = Math.Round((double)loseCount / count * 100, 2);
        var winList = userList.Where(x => x.Amount > 0).ToList();
        var lostList = userList.Where(x => x.Amount < 0).ToList();
        var winGroupList = winList.GroupBy(x => x.Number).Select(x => x.FirstOrDefault()).ToList();
        var loseGroupList = lostList.GroupBy(x => x.Number).Select(x => x.FirstOrDefault()).ToList();
        var exceptList = list.Select(x => x.Number).Except(userList.Select(x => x.Number)).Distinct().OrderBy(x => x).ToList();
        var exceptAmountList = list.Where(x => x.Amount > 0).Where(x => exceptList.Contains(x.Number)).Select(x => x.Amount).Distinct().OrderBy(x => x).ToList();
        var leopardList = userList.Where(x => x.IsSame).ToList();
        var leopardRate = Math.Round((double)leopardList.Count / count * 100, 2);

        Console.WriteLine($"您共计投掷{count}次,每次交费{perCountCost}元,花费{cost}元,游戏{(money > 0 ? $"赢得{money}元" : $"亏损{money}元")},共计{(total > 0 ? $"赢得{total}元" : $"亏损{total}元")}");
        Console.WriteLine("统计信息:");
        Console.WriteLine($"方向:{(stateInput == "1" ? "顺时针" : "逆时针")}");
        Console.WriteLine($"赢钱次数{winCount}次,胜率:{winRate}%");
        Console.WriteLine($"输钱次数{loseCount}次,败率:{loseRate}%");
        Console.WriteLine($"赢钱面额分布:{string.Join(",", userList.Where(x => x.Amount > 0).Select(x => x.Amount).Distinct())}");
        Console.WriteLine($"豹子概率:{leopardRate}%{(leopardRate == 0 ? "" : $", 出现过豹子的点数:{string.Join(",", leopardList?.Select(x => x.Values.FirstOrDefault())?.Distinct()?.OrderBy(x => x).ToList())}")}");
        Console.WriteLine($"本次从未到达过的数字集合:{string.Join(",", exceptList)}");
        Console.WriteLine($"本次从赢到过的金额集合:{string.Join(",", exceptAmountList)}");
        Console.WriteLine(splitLine);
        userList.Clear();

    PressContinue:
        Console.WriteLine("");
        Console.WriteLine("按回车继续");
        var key = Console.ReadKey();
        if (key.Key == ConsoleKey.Enter)
        {
            goto retryChooseState;
        }
        else
        {
            goto PressContinue;
        }
    }

    /// <summary>
    /// 打印画板
    /// </summary>
    public static void PrintPaint()
    {
        Console.WriteLine("+-------+-------+-------+-------+-------+-------+-------+-------+-------+");
        Console.WriteLine("|                             仅用于反诈宣传                            |");
        Console.WriteLine("+-------+-------+-------+-------+-------+-------+-------+-------+-------+");
        // 打印第一行（数字）
        Console.WriteLine("|   11  |   18  |   9   |   20  |   7   |   22  |   5   |   24  |   29  |");
        // 打印第二行（金额）
        Console.WriteLine("| 200元 | 1000元| 500元 | 1400元| 100元 | 1000元| 400元 | 1400元| 300元 |");
        // 打印分隔线
        Console.WriteLine("+-------+-------+-------+-------+-------+-------+-------+-------+-------+");
        // 打印第三行（数字）
        Console.WriteLine("|   16  |                                                       |   26  |");
        // 打印第四行（金额）                                                       
        Console.WriteLine("| 1200元|                                                       | 1200元|");
        // 打印分隔线
        Console.WriteLine("+-------+                                                       +-------+");
        // 打印第五行（数字）
        Console.WriteLine("|   13  |                                                       |   27  |");
        // 打印第六行（金额）
        Console.WriteLine("| 600元 |                                                       | -580元|");
        // 打印分隔线
        Console.WriteLine("+-------+                                                       +-------+");
        // 打印第七行（数字）
        Console.WriteLine("|   14  |                                                       |   28  |");
        // 打印第八行（金额）
        Console.WriteLine("| 1000元|                                                       | 1600元|");
        // 打印分隔线
        Console.WriteLine("+-------+                                                       +-------+");
        // 打印第九行（数字）
        Console.WriteLine("|   15  |                                                       |   25  |");
        // 打印第十行（金额）
        Console.WriteLine("| 200元 |                                                       | 300元 |");
        // 打印分隔线
        Console.WriteLine("+-------+-------+-------+-------+-------+-------+-------+-------+-------+");
        // 打印第十一行（数字）
        Console.WriteLine("|   12  |   17  |   10  |   19  |   8   |   21  |   6   |   23  |   30  |");
        // 打印第十二行（金额）
        Console.WriteLine("| 1200元| 400元 | 1000元| 300元 | 1200元| 300元 | 1400元| 200元 | 6000元|");
        // 打印表格底部边框
        Console.WriteLine("+-------+-------+-------+-------+-------+-------+-------+-------+-------+");
    }

    /// <summary>
    /// 操纵骰子移动
    /// </summary>
    /// <param name="state"></param>
    /// <param name="values"></param>
    public static void MoveNext(DirectionType state, List<int> values)
    {
        var number = values.Sum();
        var index = state == DirectionType.ClockWise ? list.IndexOf(list.FirstOrDefault(x => x.Number == number)) : reversedList.IndexOf(list.FirstOrDefault(x => x.Number == number));
        var nextIndex = 0;
        UserDiceModel result = new UserDiceModel
        {
            Values = values
        };

        //豹子
        if (values.Distinct().Count() == 1)
        {
            var value = values.FirstOrDefault();
            result.Number = value;
            result.IsSame = true;
            switch (value)
            {
                case 1:
                case 2:
                    result.Amount = 2000;
                    break;
                case 3:
                case 4:
                    result.Amount = 4000;
                    break;
                case 5:
                case 6:
                    result.Amount = 6000;
                    break;
                default:
                    break;
            }
            Console.WriteLine($"|已为您投掷骰子,分别为({string.Join(",", values)}),到达数字:{result.Number},对应金额:{result.Amount}|");
            userList.Add(result);
            return;
        }

        nextIndex = index + number;
        if (state == DirectionType.ClockWise)
        {
            if (list.Count < nextIndex)
            {
                nextIndex = nextIndex - list.Count;
            }
            result.Number = list[nextIndex - 1].Number;
            result.Amount = list[nextIndex - 1].Amount;
        }
        else if (state == DirectionType.CounterClockWise)
        {
            if (reversedList.Count < nextIndex)
            {
                nextIndex = nextIndex - reversedList.Count;
            }
            result.Number = reversedList[nextIndex - 1].Number;
            result.Amount = reversedList[nextIndex - 1].Amount;
        }
        Console.WriteLine($"|已为您投掷骰子,分别为({string.Join(",", values)}),到达数字:{result.Number},对应金额:{result.Amount}|");
        userList.Add(result);
    }
}


/// <summary>
/// 方向枚举
/// </summary>
public enum DirectionType
{
    /// <summary>
    /// 顺时针
    /// </summary>
    ClockWise = 1,
    /// <summary>
    /// 逆时针
    /// </summary>
    CounterClockWise = 2,
}

/// <summary>
/// 反转List扩展
/// </summary>
public static class ListExtensions
{
    public static List<T> ReverseCopy<T>(this List<T> list)
    {
        return list.AsEnumerable().Reverse().ToList();
    }
}

/// <summary>
/// 骰子模型
/// </summary>
public class DiceModel
{
    /// <summary>
    /// 数字号
    /// </summary>
    public int Number { get; set; }
    /// <summary>
    /// 对应金额
    /// </summary>
    public int Amount { get; set; }
}

/// <summary>
/// 用户投掷骰子模型
/// </summary>
public class UserDiceModel : DiceModel
{
    /// <summary>
    /// 记录骰子信息
    /// </summary>
    public List<int> Values { get; set; } = new List<int>();
    /// <summary>
    /// 是否是豹子
    /// </summary>
    public bool IsSame { get; set; } = false;
}