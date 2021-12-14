using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Class for saving data. An object is created and continuously updated with the values. Data can be saved periodically after each step in the experiment and returned as a string.
/// </summary>
public class SavedData
{
    // TODO (maybe): This should probably a singleton. Does not really matter though, as it is only created once by the main class, would be only for being technically correct...
    private string _starttime;
    private string _endtime;
    private int _age;
    private string _gender;
    private List<string> _visualizations = new List<string>();

    private List<string> _calculations = new List<string>();
    private List<string> _calculationsResults = new List<string>();
    private List<string> _calculationsTime = new List<string>();

    private List<string> _investements = new List<string>();
    private List<string> _investmentResults = new List<string>();
    private List<string> _investmentsTime = new List<string>();

    private bool _isComplete;

    /// <summary>
    /// Constructor, on creation at the start of the experiment. Logically, (all) data is missing at that point, so the boolean is always set to false.
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

    public void addCalculationTime(string calculationTime)
    {
        _calculationsTime.Add(calculationTime);
    }

    public void addInvestment(string investment)
    {
        _investements.Add(investment);
    }

    public void addInvestmentResult(string investmentResult)
    {
        _investmentResults.Add(investmentResult);
    }

    public void addInvestmentTime(string investmentTime)
    {
        _investmentsTime.Add(investmentTime);
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
    /// called periodically after every step. See the comment at the bottom of this file for more detailled information. To know what to save, information is transmitted
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
                builder.Append(_visualizations[_visualizations.Count - 1]).Append(","); /* Since this is called after every change in the experiment, always using the last element on the list works fine */
                break;
            case "calculation": /* For new calculation identifiers, of which there are 9 in total */
                builder.Append(_calculations[_calculations.Count - 1]).Append(",");
                break;
            case "calculationResult": /* For new calculation results, 9 in total */
                builder.Append(_calculationsResults[_calculationsResults.Count - 1]).Append(",");
                builder.Append(_calculationsTime[_calculationsTime.Count - 1]).Append(",");
                break;
            case "investment": /* For new investment identifiers, 9 in total */
                builder.Append(_investements[_investements.Count - 1]).Append(",");
                break;
            case "investmentResults": /* For new investment results, 9 in total */
                builder.Append(_investmentResults[_investmentResults.Count - 1]).Append(",");
                builder.Append(_investmentsTime[_investmentsTime.Count - 1]).Append(",");
                break;
            case "finish": /* Last piece of data, the time at the end and information that the experiment is completed */
                builder.Append(_endtime + ",");
                _isComplete = true;
                builder.Append(_isComplete.ToString());
                break;
        }
        return builder.ToString();
    }

    /// <summary>
    /// Was used to save the whole file at the end, no longer needed, since partial saving now exists.
    /// </summary>
    /// <returns></returns>
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

/* How the data is organized in the output CSV file
starttime,			    
age,				    
gender,				    
visualization1,			
calculation1,			
calculation1result,	
calculation1time,
calculation2,			
calculation2result,	
calculation2time,
calculation3,			
calculation3result,	
calculation3time,
investment1,			
investment1result,	
investment1time,
investment2,			
investment2result,
investment2time,	
investment3,			
investment3result,	
investment3time,	
visualization2,´		
calculation4,			
calculation4result,		
calculation4time,
calculation5,			
calculation5result,	
calculation5time,
calculation6,			
calculation6result,
calculation6time,
investment4,			
investment4result,	
investment4time,	
investment5,		
investment5result,
investment5time,	
investment6,			
investment6result,		
investment6time,	
visualization3,			
calculation7,			
calculation7result,	
calculation7time,
calculation8,			
calculation8result,	
calculation8time,
calculation9,			
calculation9result,	
calculation9time,
investment7,			
investment7result,	
investment7time,	
investment8,		
investment8result,		
investment8time,	
investment9,			
investment9result,	
investment9time,	
endtime,			   
isComplete			  
*/