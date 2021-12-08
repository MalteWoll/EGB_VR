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
