﻿namespace Maxfire.Skat
{
	public interface ISelvangivneBeloeb
	{
		decimal PersonligIndkomstAMIndkomst { get; }
		decimal PersonligIndkomstEjAMIndkomst { get; }
		decimal FradragPersonligIndkomst { get; }

		decimal KapitalIndkomst { get; }
		decimal FradragKapitalIndkomst { get; }

		decimal LigningsmaessigeFradragMinusBeskaeftigelsesfradrag { get; }

		decimal KapitalPensionsindskud { get; }
		decimal PrivatTegnetPensionsindskud { get; }
	}
}