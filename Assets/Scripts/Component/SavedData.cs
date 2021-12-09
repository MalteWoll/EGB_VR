using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SavedData
{
    private string _starttime;
    private string _endtime;
    private int _age;
    private string _gender;
    private List<string> _visualizations = new List<string>();
    private List<string> _calculations = new List<string>();
    private List<string> _calculationsResults = new List<string>();
    private List<string> _investements = new List<string>();
    private List<string> _investmentResults = new List<string>();
    private bool _isComplete;

    /// <summary>
    /// Constructor, on creation at the start of the experiment. Logically, data is missing, so the boolean is always set to true.
    /// </summary>
    public SavedData()
    {
        _isComplete = false;
    }

    public string Starttime
    {
        get { return _starttime; }
        set { _starttime = value; }
    }

    public string Endtime
    {
        get { return _endtime; }
        set { _endtime = value; }
    }

    public int Age
    {
        get { return _age; }
        set { _age = value; }
    }

    public string Gender
    {
        get { return _gender; }
        set { _gender = value; }
    }

    public bool MissingData
    {
        get { return _isComplete; }
        set { _isComplete = value; }
    }

    public void addVisualization(string visualization)
    {
        _visualizations.Add(visualization);
    }

    public void addCalculation(string calculation)
    {
        _calculations.Add(calculation);
    }

    public void addCalculationResult(string calculationResult)
    {
        _calculationsResults.Add(calculationResult);
    }

    public void addInvestment(string investment)
    {
        _investements.Add(investment);
    }

    public void addInvestmentResult(string investmentResult)
    {
        _investmentResults.Add(investmentResult);
    }

    public List<string> Investments
    {
        get { return _investements; }
    }

    public List<string> InvestmentResults
    {
        get { return _investmentResults; }
    }

    public List<string> Calculations
    {
        get { return _calculations; }
    }

    public List<string> CalculationResults
    {
        get { return _calculationsResults; }
    }

    /// <summary>
    /// There are 45 pieces of data that need to be saved: 3 for date and time, 2 for age and gender, 39 for the main experiment, and 1 for the complete data. To save all data, incomplete too, this function is
    /// called periodically after every step, and new data is appended to the output file. See the comment at the bottom of this file for more detailled information. To know what to save, information is transmitted
    /// as a string, and identified per switch case in this function.
    /// </summary>
    /// <returns></returns>
    public string SaveProgress(string info)
    {
        StringBuilder builder = new StringBuilder();
        switch (info)
        {
            case "initial": /* The initial save after the intro. Contains start time, age and gender */
                builder.Append("\n" + _starttime + ",").Append(_age + ",").Append(_gender + ","); /* Line break before the first line, so new set of data is always in a new line */
                break;
            case "visualization": /* For new visualizations, of which there are 3 in total */
                builder.Append(_visualizations[_visualizations.Count - 1]).Append(",");
                break;
            case "calculation": /* For new calculation identifiers, of which there are 9 in total */
                builder.Append(_calculations[_calculations.Count - 1]).Append(",");
                break;
            case "calculationResult": /* For new calculation results, 9 in total */
                builder.Append(_calculationsResults[_calculationsResults.Count - 1]).Append(",");
                break;
            case "investment": /* For new investment identifiers, 9 in total */
                builder.Append(_investements[_investements.Count - 1]).Append(",");
                break;
            case "investmentResults": /* For new investment results, 9 in total */
                builder.Append(_investmentResults[_investmentResults.Count - 1]).Append(",");
                break;
            case "finish": /* Last piece of data, the time at the end and information that the experiment is completed */
                builder.Append(_endtime + ",");
                _isComplete = true;
                builder.Append(_isComplete.ToString());
                break;
        }
        return builder.ToString();
    }

    public string CreateOutputString()
    {
        StringBuilder builder = new StringBuilder();
        // title line:
        builder.Append("starttime,endtime,age,gender,visualization,calculation1,calculation1Result,calculation2,calculation2Result,calculation3,calculation3Result,investment1,investment1result,investment2,investment2result,investmen3,investment3result");
        builder.AppendLine();

        int calcCounter = 0;
        int investCounter = 0;

        for(int i = 0; i < 3; i++)
        {
            builder.Append(_starttime).Append(",").Append(_endtime).Append(",").Append(_age).Append(",").Append(_gender).Append(",").Append(_visualizations[i]).Append(",");

            // Add the calculations and results for this visualization
            for(int j = 0; j < 3; j++)
            {
                builder.Append(_calculations[calcCounter]).Append(",").Append(_calculationsResults[calcCounter]).Append(",");
                calcCounter++;
            }

            for(int k = 0; k < 3; k++)
            {
                builder.Append(_investements[investCounter]).Append(",").Append(_investmentResults[investCounter]).Append(",");
                investCounter++;
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }
}

/*
starttime,			    1
age,				    3
gender,				    4
visualization1,			5
calculation1,			6
calculation1result,		7
calculation2,			8
calculation2result,		9
calculation3,			10
calculation3result,		11
investment1,			12
investment1result,		13
investment2,			14
investment2result,		15
investment3,			16
investment3result,		17
visualization2,´		18
calculation4,			19
calculation4result,		20
calculation5,			21
calculation5result,		22
calculation6,			23
calculation6result,		24
investment4,			25
investment4result,		26
investment5,			27
investment5result,		28
investment6,			29
investment6result,		30
visualization3,			31
calculation7,			32
calculation7result,		33
calculation8,			34
calculation8result,		35
calculation9,			36
calculation9result,		37
investment7,			38
investment7result,		39
investment8,			40
investment8result,		41
investment9,			42
investment9result,		43
endtime,			    2
isComplete			    44
*/