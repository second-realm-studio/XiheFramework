using UnityEngine;
using XiheFramework.Combat.Damage.DataTypes;

namespace XiheFramework.Combat.Damage {
    public abstract class DamageProcessorBase : MonoBehaviour {
        private DamageData m_RawDamageData;
        private DamageValidateOutputData m_ValidateOutputData;
        private DamagePreprocessOutputData m_PreprocessOutputData;
        private DamageCalculateOutputData m_CalculateOutputData;
        private DamageEventArgs m_DamageResult;

        public bool Process(DamageData damageData, out DamageEventArgs outputData) {
            m_RawDamageData = damageData;

            var valid = Validate(ref m_ValidateOutputData, m_RawDamageData);
            if (!valid) {
                outputData = new DamageEventArgs();
                return false;
            }

            Preprocess(ref m_PreprocessOutputData, m_RawDamageData, m_ValidateOutputData);
            Calculate(ref m_CalculateOutputData, m_RawDamageData, m_ValidateOutputData, m_PreprocessOutputData);
            Postprocess(ref m_DamageResult, m_RawDamageData, m_ValidateOutputData, m_PreprocessOutputData, m_CalculateOutputData);

            outputData = m_DamageResult;
            return true;
        }

        /// <summary>
        /// Validate damage data before processing
        /// </summary>
        /// <param name="outputData"></param>
        /// <param name="rawDamageData"></param>
        /// <returns></returns>
        protected abstract bool Validate(ref DamageValidateOutputData outputData, in DamageData rawDamageData);

        /// <summary>
        /// Apply Buff/Debuff/Tag modify raw damage/ raw stamina damage before feeding into damage formula during Calculate phase
        /// </summary>
        /// <param name="rawDamageData"></param>
        /// <param name="outputData"></param>
        /// <param name="validateOutput"></param>
        protected abstract void Preprocess(ref DamagePreprocessOutputData outputData, in DamageData rawDamageData, in DamageValidateOutputData validateOutput);

        /// <summary>
        /// Apply damage formula, calculate damage and stamina damage
        /// </summary>
        /// <param name="outputData"></param>
        /// <param name="rawDamageData"></param>
        /// <param name="validateOutput"></param>
        /// <param name="preprocessOutput"></param>
        protected abstract void Calculate(ref DamageCalculateOutputData outputData, in DamageData rawDamageData, in DamageValidateOutputData validateOutput,
            in DamagePreprocessOutputData preprocessOutput);

        /// <summary>
        /// Apply Buff/Debuff/Tag modify calculated damage/ calculated stamina damage after damage formula
        /// </summary>
        /// <param name="outputData"></param>
        /// <param name="rawDamageData"></param>
        /// <param name="validateOutput"></param>
        /// <param name="preprocessOutput"></param>
        /// <param name="calculateOutput"></param>
        protected abstract void Postprocess(ref DamageEventArgs outputData, in DamageData rawDamageData, in DamageValidateOutputData validateOutput,
            in DamagePreprocessOutputData preprocessOutput, in DamageCalculateOutputData calculateOutput);
    }
}