import type Siem from "./Siem";
import type { Sigma } from "./Sigma";

//Defines the structure of a rule.
export default interface Rules {
  ruleId: string;
  parentId: string;
  siems: Siem[];
  sigma: Sigma;
}
