input_vars = input_vars or {
  "UL_AB",
  "UL_BC",
  "UL_CA",
  "I_A_SEM",
  "I_B_SEM",
  "I_C_SEM",
  "PA",
  "PS",
  "COS_FI",
  "SEM_I_NOM",
  "SEM_COS_FI_NOM",
}

output_vars = output_vars or {
  "PUMP_STATE",
  "UL_MEAN",
  "UL_DIS",
  "I_MEAN_SEM",
  "I_DIS_SEM",
  "I_ACT",
  "LOAD"
}


function calc()
  UL_MEAN = ( UL_AB + UL_BC + UL_CA ) / 3
  if UL_MEAN==0 then
    UL_DIS = 0
  else
    UL_DIS = ( 100 * ( math.max( UL_AB, UL_BC, UL_CA ) - math.min( UL_AB, UL_BC, UL_CA ) ) ) / UL_MEAN
  end

  I_MEAN_SEM = ( I_A_SEM + I_B_SEM + I_C_SEM ) / 3
  if I_MEAN_SEM==0 then
    I_DIS_SEM = 0
  else
    I_DIS_SEM = ( 100 * ( math.max( I_A_SEM, I_B_SEM, I_C_SEM ) - math.min( I_A_SEM, I_B_SEM, I_C_SEM ) ) ) / I_MEAN_SEM
  end

  I_ACT = I_MEAN_SEM * COS_FI

  if OUT_FREQ > 0 and OUT_FREQ ~= math.huge then
    PUMP_STATE = 1
  else
    PUMP_STATE = 0
  end

  if SEM_I_NOM==0 or SEM_COS_FI_NOM==0 then
    LOAD = 0
  else
    LOAD = math.ceil( I_ACT * 100 / ( SEM_I_NOM * SEM_COS_FI_NOM ) )
  end
end