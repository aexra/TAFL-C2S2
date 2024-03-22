# TAFL-C2S2
Это небольшой проект, написанный на [WinUI 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/) с использованием C# и XAML, предназначенный для реализации в стиле страниц всех лабораторных работ по курсу Теории Автоматов и Формальных Языков за 4-й семестр курса Программной инженерии (09.03.04) в [ДГТУ](https://edu.donstu.ru/).

## Лабораторные
В этом проекте на момент записи README размещено 3 лабы:
1. Лексикографическое кодирование
2. Регулярные выражения
3. Детерминизация

### Лексикографическое кодирование
Представляет из себя страницу с несколькими текстовыми полями для ввода и вывода. Предоставляет возможность закодировать и декодировать данные по переданным алфавитам и словам/кодам.
![image](https://github.com/aexra/TAFL-C2S2/assets/121866384/a722354b-87f3-4cc3-ace6-2fb25b2ab28f)

### Регулярные выражения
Представляет из себя страницу с возможностью вывести m строк в лексикографическом порядке среди n глубины поиска в соответствии с введенным регулярным выражением стандарта .NET 7.0
![image](https://github.com/aexra/TAFL-C2S2/assets/121866384/8d49961e-ba26-455e-a3a0-25cad1e7d895)

### Детерминация
Представляет из себя страницу с редактором графа и возможностью вывести в другой редактор сбоку (только для чтения) детерминированный граф на основе созданного. Весь редактор графов написан вручную из головы без привлечения сторонних решений и предлагает возможность создания вершин, переименования их, удаления, соединения с другими вершинами, перемещения, удаления ребер и задания им весов.
Весь редактор графов реализован в классах CanvasedGraph, CanvasedEdge и элементе управления GraphNodeControl, отвечающего за каждую отдельную вершину.
Пример использования редактора:
```cs
public sealed partial class SamplePage : Page
{
    private readonly CanvasedGraph GraphConstructor;

    public SamplePageViewModel ViewModel
    {
        get;
    }

    public SamplePage()
    {
        ViewModel = App.GetService<SamplePageViewModel>();
        InitializeComponent();

        GraphConstructor = new(ConstructorCanvas);
    }
}
```
Подразумевается, что на странице в XAML разметке создан канвас ConstructorCanvas.

Немного скриншотов реализации:
![image](https://github.com/aexra/TAFL-C2S2/assets/121866384/f2d960a4-b53d-43e8-ac41-83be6902d3be)
![image](https://github.com/aexra/TAFL-C2S2/assets/121866384/a2e0be0f-0603-4a0b-ba33-ddd4bcccd69b)
